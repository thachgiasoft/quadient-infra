using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using Autofac;
using Autofac.Integration.Mvc;
using Infrastructure.Core.DependencyManagemenet;
using Infrastructure.Core.Fakes;
using Infrastructure.Core.TypeFinders;

namespace Infrastructure.WebFramework
{
    public class DependencyRegistrar : INeedDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //HTTP context and other related stuff
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/", new FakePrincipal(new FakeIdentity("fakeuser"), new string[] { "fakerole" }), new NameValueCollection(), new NameValueCollection(),
            new HttpCookieCollection(), new SessionStateItemCollection(), new NameValueCollection()) as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerRequest();

            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerRequest();

            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());
        }

        public int Order { get { return 1; } }
    }
}