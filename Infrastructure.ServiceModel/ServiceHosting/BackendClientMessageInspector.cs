using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.TypeFinders;
using Infrastructure.ServiceModel.ServiceHosting.Extensions;
using Infrastructure.ServiceModel.ServiceLibrary;
using System.Xml;
using System.Runtime.Serialization;

namespace Infrastructure.ServiceModel.ServiceHosting
{
    public class BackendClientMessageInspector : IClientMessageInspector
    {
        public const string TabloIliskiKeyAccessor = "3D6BEACF-7B45-4482-813C-456D1525C14D";
        private static Type _currentContextInfoProviderType;
        private readonly ServerNotification _serverNotification;

        public BackendClientMessageInspector()
        {
            if (EngineContext.Current.IsRegistered<ServerNotification>())
                _serverNotification = EngineContext.Current.Resolve<ServerNotification>();
        }
        #region IClientMessageInspector Members

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var headerIndex = reply.Headers.FindHeader(OperationInspector.ServerInfoKey, OperationInspector.ServerInfoNamespaceKey);
            if (headerIndex != -1)
            {
                var header = reply.Headers.GetHeader<ServerInfo>(headerIndex);
                if (_serverNotification != null)
                    _serverNotification.NotifyForServerChanges(new ServerNotify(header) { NotifyType = "BackendClientMessageInspector-->AfterReceiveReply" });
            }
            return;
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            if (request == null) throw new ArgumentNullException("request");
            // Service Runtime'a gececek olan kullanici, ip vs. gibi degerler bir header olarak request'e eklenecek.
            MessageBuffer buffer = request.CreateBufferedCopy(request.ToString().Length);
            Message tRequest = buffer.CreateMessage();
            ServiceContextInfo contextInfo = null;

            var containsMessageHeader = false;
            if (OperationContext.Current != null && OperationContext.Current.IncomingMessageHeaders != null)
            {
                int contextHeaderIndex = OperationContext.Current.IncomingMessageHeaders.FindHeader(ServiceBase.ContextDataKey, ServiceBase.ContextDataNamespaceKey);
                if (contextHeaderIndex >= 0)
                {
                    //contextInfo = OperationContext.Current.IncomingMessageHeaders.GetHeader<DefaultServiceContextInfo>(contextHeaderIndex);
                    var typeFinder = EngineContext.Current.Resolve<ITypeFinder>();
                    var contextInfos = typeFinder.FindClassesOfType<ServiceContextInfo>().ToList();
                    var currentContextInfo = contextInfos.Count() == 1
                                                          ? contextInfos[0]
                                                          : contextInfos.FirstOrDefault(
                                                              t => !(t == typeof(DefaultServiceContextInfo)));
                    using (XmlDictionaryReader reader = OperationContext.Current.IncomingMessageHeaders.GetReaderAtHeader(contextHeaderIndex))
                    {
                        MessageHeaderInfo headerInfo = OperationContext.Current.IncomingMessageHeaders[contextHeaderIndex];
                        var serializer = new DataContractSerializer(currentContextInfo.UnderlyingSystemType, headerInfo.Name, headerInfo.Namespace, null, int.MaxValue, false, false, null);
                        contextInfo = serializer.ReadObject(reader) as ServiceContextInfo;
                    }

                    containsMessageHeader = true;
                }
            }
            if (!containsMessageHeader)
            {
                if (_currentContextInfoProviderType == null)
                {
                    var typeFinder = EngineContext.Current.Resolve<ITypeFinder>();
                    var contextInfoProviders = typeFinder.FindClassesOfType<ServiceContextInfoProvider>().ToList();
                    _currentContextInfoProviderType = contextInfoProviders.Count() == 1
                                                          ? contextInfoProviders[0]
                                                          : contextInfoProviders.FirstOrDefault(
                                                              t => !(t == typeof(DefaultServiceContextInfoProvider)));
                }

                if (_currentContextInfoProviderType == null)
                    _currentContextInfoProviderType = typeof(DefaultServiceContextInfoProvider);
                var contextInfoProvider =
                    Activator.CreateInstance(_currentContextInfoProviderType) as ServiceContextInfoProvider;
                if (contextInfoProvider != null)
                    contextInfo = contextInfoProvider.GetServiceContextInfo();

                if (contextInfo == null) //default
                    contextInfo = new DefaultServiceContextInfo();
                contextInfo.ClientIp = "127.0.0.1";
            }

            MessageHeader header = null;
            // ---------------------------------------------------------------------------------------------------------------------------------
            header = MessageHeader.CreateHeader(ServiceBase.ContextDataKey, ServiceBase.ContextDataNamespaceKey, contextInfo);
            tRequest.Headers.Add(header);

            ClientComputerInfo clientcomputerInfo = null;

            containsMessageHeader = false;
            if (OperationContext.Current != null && OperationContext.Current.IncomingMessageHeaders != null)
            {
                int contextHeaderIndex = OperationContext.Current.IncomingMessageHeaders.FindHeader(ServiceBase.ClientComputerInfoKey, ServiceBase.ClientComputerInfoNamespaceKey);
                if (contextHeaderIndex >= 0)
                {
                    clientcomputerInfo = OperationContext.Current.IncomingMessageHeaders.GetHeader<ClientComputerInfo>(contextHeaderIndex);
                    containsMessageHeader = true;
                }
            }
            if (!containsMessageHeader)
            {
                //istek yapilan yerde bu kullanilacak
                clientcomputerInfo = new ClientComputerInfo()
                {
                    ClientComputerName = System.Net.Dns.GetHostName(),
                    ClientComputerIpAddress = ServiceBase.GetLocalIPAddress()
                };
            }

            header = MessageHeader.CreateHeader(ServiceBase.ClientComputerInfoKey, ServiceBase.ClientComputerInfoNamespaceKey, clientcomputerInfo);
            tRequest.Headers.Add(header);

            request = tRequest;
            buffer.Close();
            return null;
        }

        #endregion

    }
}
