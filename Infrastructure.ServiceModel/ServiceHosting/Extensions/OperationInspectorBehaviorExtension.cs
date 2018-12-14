using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Infrastructure.Core;
using Infrastructure.Core.Helpers;
using Infrastructure.ServiceModel.ServiceHosting.Attributes;
using Infrastructure.ServiceModel.ServiceLibrary;
using Infrastructure.Core.DependencyManagemenet;
using Autofac.Integration.Wcf;
using System.Web;

namespace Infrastructure.ServiceModel.ServiceHosting.Extensions
{
    public class OperationInspector : IDispatchMessageInspector, IServiceBehavior
    {
        public const string ServerInfoKey = "{290680D9-16B3-4C47-B40E-BF32046EC2A8}";
        public const string ServerInfoNamespaceKey = "http://ServerInfo/Context";
        public OperationInspector()
        {

        }
        /// <summary>
        /// 2. olarak çağrılır
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        /// <param name="endpoints"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            return;
        }
        /// <summary>
        /// 3. olarak çağırılır.
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher chDispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher epDispatcher in chDispatcher.Endpoints)
                {
                    epDispatcher.DispatchRuntime.MessageInspectors.Add(this);
                    foreach (DispatchOperation op in epDispatcher.DispatchRuntime.Operations)
                        op.ParameterInspectors.Add(new ParameterInspector());
                }
            }
        }
        /// <summary>
        /// 1. olarak çağırılır
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            return;
        }


        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            var cinfo = new ClientInfo();

            //Collect any info that passed in the headers from Client, if any.

            cinfo.ServerTimeStamp = DateTime.Now; //Timestamp after the call is received.
            cinfo.Platform = "WCF";
            OperationDescription operationDesc = GetOperationDescription(OperationContext.Current);
            if (operationDesc != null)
            {
                Type contractType = operationDesc.DeclaringContract.ContractType;
                cinfo.Action = operationDesc.Name;
                cinfo.TypeName = contractType.FullName;
                cinfo.AssemblyName = contractType.Assembly.GetName().Name;
                var syncMethod = operationDesc.SyncMethod;
                AuditDataChanges[] attr = null;
                // Eger OperationContract te verilen Method Name ile methodun orjinal adi farkli ise bu kontrol gerekiyor. yoksa methodu bulamiyor.
                var methods = contractType.GetMethods();
                if (!operationDesc.Name.Equals(syncMethod.Name) || methods.Count() > 1)
                {
                    var mi = methods.First(method => method.Name == syncMethod.Name && !method.IsGenericMethod && CommonHelper.ArraysEqual(method.GetParameters(), syncMethod.GetParameters()));
                    if (mi != null)
                        attr = mi.GetCustomAttributes(typeof(AuditDataChanges), false) as AuditDataChanges[];
                }
                else
                    attr = contractType.GetMethod(operationDesc.Name).GetCustomAttributes(typeof(AuditDataChanges), false) as AuditDataChanges[];

                if (attr != null && attr.Length > 0)
                {
                    cinfo.IsLogEnable = true;
                    ServiceBase.AuditEnable = true;
                }
            }

            cinfo.ServerName = Dns.GetHostName();
            cinfo.ServerProcessName = System.AppDomain.CurrentDomain.FriendlyName;

            if (OperationContext.Current != null)
            {
                var index = OperationContext.Current.IncomingMessageHeaders.FindHeader(ServiceBase.ClientComputerInfoKey, ServiceBase.ClientComputerInfoNamespaceKey);

                if (index > 0)
                {
                    var clientComputerInfo = OperationContext.Current.IncomingMessageHeaders.GetHeader<ClientComputerInfo>(index);
                    cinfo.ClientComputerInfo = clientComputerInfo;
                }
            }

            var messageProperty = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            if (messageProperty != null) cinfo.ClientIp = messageProperty.Address;
            ServiceBase.ClientInfo = cinfo;

            return cinfo;
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            //correlationState is of type ClientInformation and it is set in AfterReceiveRequest event.
            if (correlationState != null)
            {
                var cinfo = correlationState as ClientInfo;
                if (cinfo != null)
                {
                    cinfo.ServerEndTimeStamp = DateTime.Now;

                    //It's okay to read the RequestMessage since the operation 
                    //has been completed and message is serialized from stream 
                    //(....stream...).
                    //RequestMessage on OperationContext is short lived. 
                    //It would be too late to serialize the RequestMessage 
                    //in another thread (exception raised: Message is closed)
                    //cinfo.Request = OperationContext.Current.RequestContext.RequestMessage.ToString();

                    //Message can be read only once. 
                    //Create a BufferedCopy of the Message and be sure to set 
                    //the original message set to a value wich has not been 
                    //copied nor read.
                    // MessageBuffer mb = reply.CreateBufferedCopy(int.MaxValue);
                    //Message responseMsg = mb.CreateMessage();
                    // reply = mb.CreateMessage();
                    //cinfo.Response = responseMsg.ToString();
                    var info = HybridContext.Current[ServerInfoKey];
                    if (reply != null && info != null)
                    {
                        var header = MessageHeader.CreateHeader(ServerInfoKey, ServerInfoNamespaceKey, info);
                        reply.Headers.Add(header);
                    }
                    if (reply != null && reply.IsFault == true)
                    {
                        cinfo.IsError = true;
                    }

                    //Log cinfo async here;
                }
            }
        }
        private OperationDescription GetOperationDescription(OperationContext operationContext)
        {
            OperationDescription od = null;
            string bindingName = operationContext.EndpointDispatcher.ChannelDispatcher.BindingName;
            string methodName = string.Empty;
            if (bindingName.Contains("WebHttpBinding"))
            {
                //REST request
                methodName = (string)operationContext.IncomingMessageProperties["HttpOperationName"];
            }
            else
            {
                //SOAP request
                string action = operationContext.IncomingMessageHeaders.Action;
                var firstOrDefault = operationContext.EndpointDispatcher.DispatchRuntime.Operations.FirstOrDefault(o => o.Action == action);
                if (
                    firstOrDefault != null)
                    methodName = firstOrDefault.Name;
            }

            EndpointAddress epa = operationContext.EndpointDispatcher.EndpointAddress;
            ServiceDescription hostDesc = operationContext.Host.Description;
            ServiceEndpoint ep = hostDesc.Endpoints.Find(epa.Uri);

            if (ep != null)
            {
                od = ep.Contract.Operations.Find(methodName);
            }

            return od;
        }

    }
    [Serializable]
    public class ClientInfo
    {
        public ClientInfo()
        {
            ServerEndTimeStamp = DateTime.Now;
            ServerTimeStamp = DateTime.Now;
            Platform = string.Empty;
            Action = string.Empty;
            TypeName = string.Empty;
            AssemblyName = string.Empty;
            ServerName = string.Empty;
            ServerProcessName = string.Empty;
            Request = string.Empty;
            Response = string.Empty;
        }
        public DateTime ServerEndTimeStamp { get; set; }

        public DateTime ServerTimeStamp { get; set; }

        public string Platform { get; set; }

        public string Action { get; set; }

        public string TypeName { get; set; }

        public string AssemblyName { get; set; }

        public string ServerName { get; set; }

        public string ServerProcessName { get; set; }

        public string Request { get; set; }

        public string Response { get; set; }

        public bool IsError { get; set; }

        public bool IsLogEnable { get; set; }

        public string ClientIp { get; set; }

        public ClientComputerInfo ClientComputerInfo { get; set; }
    }

    public class OperationInspectorBehaviorExtension : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(OperationInspector); }
        }

        protected override object CreateBehavior()
        {
            return new OperationInspector();
        }
    }
}
