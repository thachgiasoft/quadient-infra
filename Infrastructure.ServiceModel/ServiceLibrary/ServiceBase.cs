using System;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Core;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Cryptography;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.Logging;
using Infrastructure.ServiceModel.ServiceHosting;
using Infrastructure.ServiceModel.ServiceHosting.Extensions;
using System.Linq;

namespace Infrastructure.ServiceModel.ServiceLibrary
{
    public abstract class ServiceBase : IDisposable
    {
        public const string DefaultBindingName = "EBakanlikCustomBinding";
        public const string DefaultLoggerKey = "ServiceLog";
        private const string AuditEnableKey = "{D1938702-D3F2-4FB5-AD8C-95E3A253E679}";
        public const string ClientInfoKey = "{FDCE0331-4AE0-4E03-9CE9-5E021607B25D}";

        /// <summary>
        /// Bu info distributed mimarilerde calismalarda load balancerdan onceki sunucunun bilgilerini icerir
        /// </summary>
        public const string ClientComputerInfoKey = "ClientComputerInfo";

        public const string ContextDataKey = "ServiceContextInfo";
        public const string ContextDataNamespaceKey = "http://ServiceContextInfo/Context";
        public const string ClientComputerInfoNamespaceKey = "http://ServiceContextInfo/ClientComputerInfo";
        private static string _localIpAddress;

        public virtual void Dispose()
        {
            //release resources
        }

        protected ICoreCryptography Cryptography
        {
            get { return Engine.Resolve<ICoreCryptography>(); }
        }

        protected ILogger ErrorLogger
        {
            get
            {
                return
                    Engine.Resolve<ILogger>(Engine.IsRegistered<ILogger>(DefaultLoggerKey)
                                                ? DefaultLoggerKey
                                                : CoreConsts.DefaultLoggerKey);
            }
        }

        protected IEngine Engine
        {
            get { return EngineContext.Current; }
        }

        protected virtual T GetServiceContextInfo<T>() where T : ServiceContextInfo
        {
            var retval = default(T);
            if (OperationContext.Current != null)
            {
                var contextHeaderIndex =
                    OperationContext.Current.IncomingMessageHeaders.FindHeader(
                        ContextDataKey,
                        ContextDataNamespaceKey);
                if (contextHeaderIndex > 0)
                {
                    retval = OperationContext.Current.IncomingMessageHeaders.GetHeader<T>(contextHeaderIndex);
                }
            }
            return retval;
        }

        public static bool AuditEnable
        {
            get { return HybridContext.Current[AuditEnableKey] as bool? ?? false; }
            set { HybridContext.Current[AuditEnableKey] = value; }
        }

        public static ClientInfo ClientInfo
        {
            get
            {
                var info = HybridContext.Current[ClientInfoKey] as ClientInfo;
                if (info == null)
                    HybridContext.Current[ClientInfoKey] = info = new ClientInfo();
                return info;
            }
            set { HybridContext.Current[ClientInfoKey] = value; }
        }

        protected void NotifyClient(ServerInfo info)
        {
            HybridContext.Current[OperationInspector.ServerInfoKey] = info;
        }

        public static string GetLocalIPAddress()
        {
            if (!string.IsNullOrEmpty(_localIpAddress))
                return _localIpAddress;

            foreach (
                System.Net.NetworkInformation.NetworkInterface ni in
                    System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                          .Where(n => n.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up))
            {
                if (ni.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Ethernet)
                //kablolu baglanti olacak
                {
                    var addr = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                    if (addr != null) //virtual olanların gatewayi olmaz
                    {
                        foreach (
                            System.Net.NetworkInformation.UnicastIPAddressInformation ip in
                                ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            //ipv4 lu ip almis olacak . ipv6 olursa ilerde guncelleriz
                            {
                                _localIpAddress = ip.Address.ToString();
                                break;
                            }
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(_localIpAddress))
                throw new Exception("Local IP Address Not Found!");
            return _localIpAddress;
        }
    }
}

