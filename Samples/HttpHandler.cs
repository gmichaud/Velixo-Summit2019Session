using Autofac;
using PX.Data.DependencyInjection;
using PX.Export.Authentication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Velixo.Summit2019Samples
{
    public class HttpHandlerServiceRegistration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //This is an Autofac module that will be auto-discovered during application initialization.
            //We register all the components that we need
            builder.RegisterGeneric(typeof(RouteHandler<>)).SingleInstance();
            builder.RegisterType<SampleWebHookRequestHandler>().SingleInstance();
            builder.ActivateOnApplicationStart<RouteInitializer>(e => e.InitializeRoutes());

            builder
                .RegisterInstance(new LocationSettings
                {
                    Path = "/" + RouteInitializer.SampleRoute,
                    Providers =
                    {
                        new ProviderSettings
                        {
                            Name = "basic",
                            Type = typeof (BasicAuthenticationModule).AssemblyQualifiedName //BasicAuthenticationModule == username+password
                        }
                    }
                });
        }
    }

    public class RouteInitializer
    {
        private readonly ILifetimeScope _container;
        internal const string SampleRoute = "SampleWebHook/test";

        public RouteInitializer(ILifetimeScope container)
        {
            _container = container;
        }

        public void InitializeRoutes()
        {
            //This class will be called on applicatation start and will be used to register our HTTP routes
            RouteTable.Routes.Add(new Route($"{SampleRoute}", _container.Resolve<RouteHandler<SampleWebHookRequestHandler>>()));
        }
    }

    internal class RouteHandler<T> : IRouteHandler 
        where T : IHttpHandler
    {
        private readonly ILifetimeScope _container;

        public RouteHandler(ILifetimeScope container)
        {
            _container = container;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return _container.Resolve<T>();
        }
    }

    internal class SampleWebHookRequestHandler : IHttpHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Write("<H1>Hello, world!</H1>");
            context.Response.Write("Request: " + context.Request.QueryString.ToString());
            context.Response.StatusCode = 200;
            context.Response.End();

            //This code is running inside the Acumatica application domain; you can therefore interact with a graph...
        }
    }
}