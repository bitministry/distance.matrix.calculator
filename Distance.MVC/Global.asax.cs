using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using Autofac;
using Distance.Business;
using Distance.Business.Entitiy;
using Distance.Business.Interfaces;
using Distance.EntityFrame;
using Distance.EntityFrame.Repository;
using Distance.GoogleConsumer;
using Distance.MVC.App_Start;

namespace Distance.MVC
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            

            ConfigureDependencies();

            Geocoder.GoogleApiKey = ConfigurationManager.AppSettings["Google.maps.geocoding.apiKey"];
            Geocoder.WaitAfterRequest = int.Parse( ConfigurationManager.AppSettings["Google.Geocoding.WaitAfterRequest"] );  
                
            InitializeEnterpriseRootContact (); 
            Statics.CurrentCulture = new CultureInfo("en-GB");

        }

        void InitializeEnterpriseRootContact()
        {
            var contactRepository= Container.Resolve<IGenericRepository<Contact>>(); 
            var enterprise = contactRepository
                .Find(c => c.ContactType == ContactType.Enterprise)
                .Include( x=> x.Addresses )
                .FirstOrDefault();

            if (enterprise == null)
            {
                enterprise = new Contact() { Name = "EnterpriseRootContact", ContactType = ContactType.Enterprise };
                contactRepository.Save(enterprise);
            }

            Statics.UpdateEnterpriseRootContact( enterprise );
        }

        protected void Application_BeginRequest()
        {
            Request.RequestContext.HttpContext.Items.Add("autofac", Container.BeginLifetimeScope());
        }

        private void ConfigureDependencies()
        {

            var containerBuilder = new ContainerBuilder();

            var nameOrConnectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

            containerBuilder.Register(x=>new EfUnitOfWork(nameOrConnectionString))
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<GenericRepository<Address>>()
                .As<IGenericRepository<Address>>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<GenericRepository<Contact>>()
                .As<IGenericRepository<Contact>>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<GenericRepository<DistanceComparison>>()
                .As<IGenericRepository<DistanceComparison>>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<GenericRepository<ComparisonReport>>()
                .As<IGenericRepository<ComparisonReport>>().InstancePerLifetimeScope();
            Container = containerBuilder.Build();

        }

        public static IContainer Container { get; set; }
    }
}