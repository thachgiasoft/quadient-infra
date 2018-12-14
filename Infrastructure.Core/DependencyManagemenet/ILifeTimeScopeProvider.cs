using Autofac;
using Infrastructure.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.DependencyManagemenet
{
    public interface ILifeTimeScopeProvider
    {
        ILifetimeScope GetLifeTimeScope(CoreConfiguration configuration);
    }
}
