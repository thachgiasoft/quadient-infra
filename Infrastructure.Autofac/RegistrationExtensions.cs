using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Autofac;
using Autofac.Integration.Wcf;
using Infrastructure.ServiceModel;
using Infrastructure.ServiceModel.ServiceLibrary;

namespace Infrastructure.Autofac
{
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Verilen service contract ve endpointbase adrese gore servisi autofac a kayit eder
        /// </summary>
        /// <typeparam name="TServiceContract">Service Contract</typeparam>
        /// <param name="builder">Autofac Builder</param>
        /// <param name="delegate">action to get base endpoint</param>
        public static void RegisterWcfService<TServiceContract>(this ContainerBuilder builder,
                                                                Func<IComponentContext, string> @delegate)
        {
            builder.RegisterWcfService<TServiceContract>(@delegate, string.Empty);
        }
        /// <summary>
        /// Verilen service contract ve endpointbase adrese gore servisi autofac a kayit eder
        /// </summary>
        /// <typeparam name="TServiceContract">Service Contract</typeparam>
        /// <param name="builder">Autofac Builder</param>
        /// <param name="delegate">action to get base endpoint</param>
        /// <param name="key">Register key add ile register edilmek istedndiğinde </param>
        public static void RegisterWcfService<TServiceContract>(this ContainerBuilder builder, Func<IComponentContext, string> @delegate, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                builder.Register(
                    c => new ServiceChannelFactory<TServiceContract>(new CustomBinding(ServiceBase.DefaultBindingName),
                                                                     new EndpointAddress(string.Format("{0}/{1}.svc",
                                                                                                       @delegate(c),
                                                                                                       typeof(
                                                                                                           TServiceContract
                                                                                                           ).FullName))))
                       .As<IServiceChannelFactory<TServiceContract>>()
                       .SingleInstance()
                       .PreserveExistingDefaults();
                builder.Register(c => c.Resolve<IServiceChannelFactory<TServiceContract>>().CreateChannel())
                       .As<TServiceContract>()
                       .UseWcfSafeRelease()
                       .PreserveExistingDefaults();
            }
            else
            {
                builder.Register(
                   c => new ServiceChannelFactory<TServiceContract>(new CustomBinding(ServiceBase.DefaultBindingName),
                                                                    new EndpointAddress(string.Format("{0}/{1}.svc",
                                                                                                      @delegate(c),
                                                                                                      typeof(
                                                                                                          TServiceContract
                                                                                                          ).FullName))))
                      .Keyed<IServiceChannelFactory<TServiceContract>>(key)
                      .SingleInstance()
                      .PreserveExistingDefaults();
                builder.Register(c => c.ResolveKeyed<IServiceChannelFactory<TServiceContract>>(key).CreateChannel())
                       .Keyed<TServiceContract>(key)
                       .UseWcfSafeRelease()
                       .PreserveExistingDefaults();
            }
        }

        /// <summary>
        /// Verilen service contract ve endpointbase adrese gore servisi autofac a kayit eder
        /// </summary>
        /// <typeparam name="TServiceContract">Service Contract</typeparam>
        /// <param name="builder">Autofac Builder</param>
        /// <param name="baseEndPointAddress">base endpoint address</param>
        public static void RegisterWcfService<TServiceContract>(this ContainerBuilder builder,
                                                                string baseEndPointAddress)
        {
            builder.RegisterWcfService<TServiceContract>(baseEndPointAddress, string.Empty);
        }

        /// <summary>
        /// Verilen service contract ve endpointbase adrese gore servisi autofac a kayit eder
        /// </summary>
        /// <typeparam name="TServiceContract">Service Contract</typeparam>
        /// <param name="builder">Autofac Builder</param>
        /// <param name="baseEndPointAddress">base endpoint address</param>
        /// <param name="key"></param>
        public static void RegisterWcfService<TServiceContract>(this ContainerBuilder builder, string baseEndPointAddress, string key)
        {
            RegisterWcfService<TServiceContract>(builder, baseEndPointAddress, null, null, key);
        }

        /// <summary>
        /// Verilen service contract ve endpointbase adrese, binding ve EndpointAddres e gore gore servisi autofac a kayit eder
        /// </summary>
        /// <typeparam name="TServiceContract">Service Contract</typeparam>
        /// <param name="builder">Autofac builder</param>
        /// <param name="baseEndPointAddress">Endpoint address</param>
        /// <param name="binding">Binding</param>
        /// <param name="address">address</param>
        public static void RegisterWcfService<TServiceContract>(this ContainerBuilder builder,
                                                                string baseEndPointAddress, Binding binding,
                                                                EndpointAddress address)
        {
            builder.RegisterWcfService<TServiceContract>(baseEndPointAddress, binding, address, string.Empty);
        }
        /// <summary>
        /// Verilen service contract ve endpointbase adrese, binding ve EndpointAddres e gore gore servisi autofac a kayit eder
        /// </summary>
        /// <typeparam name="TServiceContract">Service Contract</typeparam>
        /// <param name="builder">Autofac builder</param>
        /// <param name="baseEndPointAddress">Endpoint address</param>
        /// <param name="binding">Binding</param>
        /// <param name="address">EndpointAddress</param>
        /// <param name="key"></param>
        public static void RegisterWcfService<TServiceContract>(this ContainerBuilder builder, string baseEndPointAddress, Binding binding, EndpointAddress address, string key)
        {
            if (binding == null || address == null)
            {
                binding = new CustomBinding(ServiceBase.DefaultBindingName);
                address =
                    new EndpointAddress(string.Format("{0}/{1}.svc", baseEndPointAddress,
                                                      typeof(TServiceContract).FullName));
            }
            if (string.IsNullOrEmpty(key))
            {
                builder.Register(c => new ServiceChannelFactory<TServiceContract>(binding, address))
                       .As<IServiceChannelFactory<TServiceContract>>()
                       .SingleInstance()
                       .PreserveExistingDefaults();
                builder.Register(c => c.Resolve<IServiceChannelFactory<TServiceContract>>().CreateChannel())
                       .As<TServiceContract>()
                       .UseWcfSafeRelease()
                       .PreserveExistingDefaults();
            }
            else
            {
                builder.Register(c => new ServiceChannelFactory<TServiceContract>(binding, address))
                      .Keyed<IServiceChannelFactory<TServiceContract>>(key)
                      .SingleInstance()
                      .PreserveExistingDefaults();
                builder.Register(c => c.ResolveKeyed<IServiceChannelFactory<TServiceContract>>(key).CreateChannel())
                       .Keyed<TServiceContract>(key)
                       .UseWcfSafeRelease()
                       .PreserveExistingDefaults();
            }
        }
    }
}
