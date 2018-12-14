using Autofac.Integration.Web;
using System.Web;

namespace Infrastructure.Autofac.WebForm
{
    public class DefaultHttpAppContainerProvider
    {
        private static IContainerProvider _containerProvider;

        static DefaultHttpAppContainerProvider()
        {
            var cpa = (IContainerProviderAccessor)HttpContext.Current.ApplicationInstance;
            _containerProvider = cpa.ContainerProvider;
        }
        public static IContainerProvider Current
        {
            get { return _containerProvider; }
        }
    }
}
