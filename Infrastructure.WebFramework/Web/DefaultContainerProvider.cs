using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core.Infrastructure;

namespace Infrastructure.WebFramework.Web
{
    public class DefaultContainerProvider
    {
        private readonly Autofac.Integration.Web.IContainerProvider _containerProvider;

        public DefaultContainerProvider()
        {
            var container = EngineContext.Current.ContainerManager.Container;
            _containerProvider = new Autofac.Integration.Web.ContainerProvider(container);
        }
        public Autofac.Integration.Web.IContainerProvider ContainerProvider
        {
            get { return _containerProvider; }
        }
    }
}
