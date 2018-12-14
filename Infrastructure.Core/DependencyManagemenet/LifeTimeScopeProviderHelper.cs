using Autofac;
using Infrastructure.Core.Configuration;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.TypeFinders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.DependencyManagemenet
{
    /// <summary>
    /// Uygulamanin life time scope provider bilgisine erisimi saglayan helper sinif.
    /// </summary>
    public class LifeTimeScopeProviderHelper
    {
        private static ILifeTimeScopeProvider _provider;

        /// <summary>
        /// Uygulamanın lifetime scope provider bilgisini donduren static metottur.
        /// </summary>
        /// <param name="configuration">Configurasyon dosyasi</param>
        /// <returns>Autofac LifetimeScope instance ı dondurur.</returns>
        public static ILifetimeScope GetLifeTimeScope(CoreConfiguration configuration)
        {
            if (_provider == null)
            {
                var typeFinderInstance = new WebAppTypeFinder(configuration);
                foreach (var item in typeFinderInstance.FindClassesOfType<ILifeTimeScopeProvider>())
                {
                    var provider = (ILifeTimeScopeProvider)Activator.CreateInstance(item);
                    var scope = provider.GetLifeTimeScope(configuration);
                    if (scope != null)
                    {
                        _provider = provider;
                        EventLog.WriteEntry("Application",string.Format("{0} Uygulamasi Icin Kullanilacak LifeTimeScopeProvider {1}", System.AppDomain.CurrentDomain.FriendlyName, item.FullName), EventLogEntryType.Information, 9500);
                        return scope;
                    }
                }
            }
            else
                return _provider.GetLifeTimeScope(configuration);
            return null;
        }
    }
}
