using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Infrastructure.ServiceModel.ServiceHosting.Behaviors;

namespace Infrastructure.ServiceModel
{
    public class ServiceChannelFactory<TServiceContract> : IServiceChannelFactory<TServiceContract>
    {
        private readonly ServiceEndpoint _serviceEndpoint;
        private readonly ChannelFactory<TServiceContract> _channelFactory;
        public ServiceChannelFactory(Binding binding, EndpointAddress address)
        {
            _serviceEndpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(TServiceContract)), binding, address);
            _serviceEndpoint.Behaviors.Add(new BackendClientMessageInspectorBehavior());
            foreach (OperationDescription op in _serviceEndpoint.Contract.Operations)
            {
                var dataContractBehavior = op.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (dataContractBehavior != null)
                {
                    dataContractBehavior.MaxItemsInObjectGraph = 2147483647;
                }
            }
            _channelFactory = new ChannelFactory<TServiceContract>(_serviceEndpoint);
        }
        public TServiceContract CreateChannel()
        {
           return _channelFactory.CreateChannel();
        }
    }
}
