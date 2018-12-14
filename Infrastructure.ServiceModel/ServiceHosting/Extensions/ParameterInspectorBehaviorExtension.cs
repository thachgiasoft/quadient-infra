using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ServiceModel.ServiceHosting.Extensions
{
    public class ParameterInspector : IParameterInspector, IServiceBehavior
    {
        public object BeforeCall(string operationName, object[] inputs)
        {
            // throw new NotImplementedException();
            return null;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// ilk cagrilir
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // throw new NotImplementedException();
        }
        /// <summary>
        /// 2. olarak cagirlir.
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        /// <param name="endpoints"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
                                         BindingParameterCollection bindingParameters)
        {
            // throw new NotImplementedException();
        }
        /// <summary>
        /// 3. cagrilir
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            //throw new NotImplementedException();
        }
        private OperationDescription GetOperationDescription(OperationContext operationContext)
        {
            OperationDescription od = null;
            string bindingName = operationContext.EndpointDispatcher.ChannelDispatcher.BindingName;
            string methodName;
            if (bindingName.Contains("WebHttpBinding"))
            {
                //REST request
                methodName = (string)operationContext.IncomingMessageProperties["HttpOperationName"];
            }
            else
            {
                //SOAP request
                string action = operationContext.IncomingMessageHeaders.Action;
                methodName = operationContext.EndpointDispatcher.DispatchRuntime.Operations.FirstOrDefault(o => o.Action == action).Name;
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
    public class ParameterInspectorBehaviorExtension : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(ParameterInspector); }
        }

        protected override object CreateBehavior()
        {
            return new ParameterInspector();
        }
    }
}
