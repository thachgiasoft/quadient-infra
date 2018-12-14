using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Infrastructure.WebFramework.Mvc.Routes
{
    public interface IRouteProvider
    {
        void RegisterRoutes(RouteCollection routes);

        int Priority { get; }

        string AppName { get; }
        string AppDescription { get; }
        string AppAssemblyName { get; }
        string AppUrl { get; }
        string [] AppMetaData { get; set; }
    }
}
