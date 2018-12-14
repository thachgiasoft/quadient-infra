using Infrastructure.Core.DependencyManagemenet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using System.Web;
using Infrastructure.Core.Configuration;

namespace Infrastructure.Autofac.WebForm
{
    public class WebFormLifeTimeScopeProvider : ILifeTimeScopeProvider
    {
        /// <summary>
        /// Verilen configurasyondaki uygulama turu webform ise Autofac lifetime scope bilgisini dondurur. Webform degilse null doner.
        /// </summary>
        /// <param name="configuration">Core configuration</param>
        /// <returns>ILifetimeScope instance dondurur.</returns>
        public ILifetimeScope GetLifeTimeScope(CoreConfiguration configuration)
        {
            if (HttpContext.Current != null && configuration.ApplicationType == ApplicationType.WebForm)
                return DefaultHttpAppContainerProvider.Current.RequestLifetime;
            return null;
        }
    }
}
