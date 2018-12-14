using Autofac;
using Autofac.Core.Lifetime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core.Configuration;

namespace Infrastructure.Core.DependencyManagemenet
{
    public class StaticAppLifetimeScopeProvider
    {
        private static CoreConfiguration _configuration;

        public static ILifetimeScope GetLifetimeScope(CoreConfiguration configuration, ILifetimeScope container)
        {
            _configuration = configuration;
            //little hack here to get dependencies when HttpContext is not available
            if (HybridContext.Current != null)
            {
                return LifetimeScope ?? (LifetimeScope = InitializeLifetimeScope(container));
            }
            return InitializeLifetimeScope(container);
        }

        static ILifetimeScope LifetimeScope
        {
            get
            {
                return (ILifetimeScope)HybridContext.Current[typeof(ILifetimeScope)];
            }
            set
            { //sadece static lifetime scope icin bu event i kullaniyoruz.
                value.CurrentScopeEnding += Value_CurrentScopeEnding;
                HybridContext.Current[typeof(ILifetimeScope)] = value;
            }
        }
        /// <summary>
        /// Bu event in fire olmasi icin asagidaki gibi bir kullanim olmasi lazim
        ///     using (EngineContext.Current.ContainerManager.Scope())
        ///     {
        ///        var sampleResolve = EngineContext.Current.Resolve<SomeComponent>();
        ///     }
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Value_CurrentScopeEnding(object sender, LifetimeScopeEndingEventArgs e)
        {
            //burada hashtable uretildigini bildigimiz icin null ataması yapiyoruz ki sonradan yeni thread geldiğinde tekrar uretilsin.
            HybridContext.Current[typeof(ILifetimeScope)] = null;
        }

        static ILifetimeScope InitializeLifetimeScope(ILifetimeScope container)
        {
            StackFrame[] frames = new StackTrace().GetFrames();
            //EventLog.WriteEntry("Application", string.Format("Autofac StaticAppLifetimeScopeProvider araciligi ile lifetime scope üretiyor. Bu scope u ureten assembly : {0}. Bunu ureten bir backgorund job veya windows uygulamasi degilse dikkate alinmasi gerekir.", (from f in frames select f.GetMethod().ReflectedType.Assembly
            //             ).Distinct().Last()), EventLogEntryType.Warning);

            if (_configuration.ApplicationType == ApplicationType.WebForm || _configuration.ApplicationType == ApplicationType.WebMVC)
                return container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag); //eger web projesi ise diger lifetime scope tag ler ile uyussun diye boyle yaptik
            return container.BeginLifetimeScope();
        }
    }
}