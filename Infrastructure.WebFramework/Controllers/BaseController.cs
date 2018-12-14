using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Infrastructure.Core.Infrastructure;
using Infrastructure.WebFramework.Filters;
using Infrastructure.WebFramework.UI.Interactions;

namespace Infrastructure.WebFramework.Controllers
{
    [SetCurrentController]
    public abstract class BaseController : Controller
    {
        protected IMessageBoxInteractions MessageBox
        {
            get { return EngineContext.Current.Resolve<IMessageBoxInteractions>(); }
        }
        public static Controller CurrentController
        {
            get { return System.Web.HttpContext.Current.Items["CurrentController"] as Controller; }
        }
    }
}
