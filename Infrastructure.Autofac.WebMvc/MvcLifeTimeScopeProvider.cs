using Infrastructure.Core.DependencyManagemenet;
using Autofac;
using Infrastructure.Core.Configuration;
using System.Web;
using Autofac.Integration.Mvc;

namespace Infrastructure.Autofac.WebMvc
{
    public class MvcLifeTimeScopeProvider : ILifeTimeScopeProvider
    {
        /// <summary>
        /// Verilen configurasyondaki uygulama turu mvc ise Autofac lifetime scope bilgisini dondurur. mvc degilse null doner.
        /// </summary>
        /// <param name="configuration">Core configuration</param>
        /// <returns>ILifetimeScope instance dondurur.</returns>
        public ILifetimeScope GetLifeTimeScope(CoreConfiguration configuration)
        {
            if (HttpContext.Current != null && configuration.ApplicationType == ApplicationType.WebMVC)
                return AutofacDependencyResolver.Current.RequestLifetimeScope;
            return null;
        }
    }
}
