using Infrastructure.Core.Configuration;
using Infrastructure.Core.DependencyManagemenet;
using System;

namespace Infrastructure.Core.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the 
    /// various services composing the core engine. Edit functionality, modules
    /// and implementations access most core functionality through this 
    /// interface.
    /// </summary>
    public interface IEngine
    {
        ContainerManager ContainerManager { get; }

        /// <summary>
        /// Initialize components and plugins in the core environment.
        /// </summary>
        /// <param name="config">Config</param>
        void Initialize(CoreConfiguration config);

        T Resolve<T>(string key="") where T : class;

        object Resolve(Type type);

        bool TryResolve(Type serviceType, out object instance);

        bool IsRegistered<T>(string key = "");

        T[] ResolveAll<T>();
    }
}
