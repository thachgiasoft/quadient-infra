using System;
using System.Collections;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Infrastructure.ServiceModel.ServiceHosting.Extensions
{
    public class WsdlExportExtension : IEndpointBehavior, IWsdlExportExtension
    {
        public void ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
        }

        public void ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
            if (context.Endpoint != null && context.Endpoint.Address.Uri.Scheme.Equals("net.tcp"))
            {
                context.WsdlPort.Service.Ports.Remove(context.WsdlPort);
            }
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }
    }
    public class WsdlExportBehaviorExtension : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(WsdlExportExtension); }
        }

        protected override object CreateBehavior()
        {
            return new WsdlExportExtension();
        }
    }
}
