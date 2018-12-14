using Infrastructure.Core.DependencyManagemenet;
using Autofac;
using Infrastructure.Core.Configuration;
using System.ServiceModel;
using Autofac.Integration.Wcf;

namespace Infrastructure.Autofac
{
    public class WcfLifeTimeScopeProvider : ILifeTimeScopeProvider
    {
        /// <summary>
        /// Verilen configurasyondaki uygulama turu wcf ise Autofac lifetime scope bilgisini dondurur. Wcf degilse null doner.
        /// </summary>
        /// <param name="configuration">Core configuration</param>
        /// <returns>ILifetimeScope instance dondurur.</returns>
        public ILifetimeScope GetLifeTimeScope(CoreConfiguration configuration)
        {
            if (OperationContext.Current != null && configuration.ApplicationType == ApplicationType.WCF)
                return AutofacInstanceContext.Current.OperationLifetime;
            return null;
        }
    }
}
