using Autofac;
using Distance.Business.Entitiy;
using Distance.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distance.MVC.Helpers
{
    [MyAuthorize]
    [HandleError]
    public class MyController : Controller
    {
        protected IGenericRepository<Contact> _contactRepository;
        protected IGenericRepository<Address> _addressRepository;
        protected IGenericRepository<DistanceComparison> _discoRepository;
        protected IGenericRepository<ComparisonReport> _reportRepository;

        protected ILifetimeScope _scope;

        public MyController()
            :base ()
        { 
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _scope = Request.RequestContext.HttpContext.Items["autofac"] as ILifetimeScope;

            _discoRepository = _scope.Resolve<IGenericRepository<DistanceComparison>>();
            _reportRepository = _scope.Resolve<IGenericRepository<ComparisonReport>>();
            _contactRepository = _scope.Resolve<IGenericRepository<Contact>>();
            _addressRepository = _scope.Resolve<IGenericRepository<Address>>();

            base.OnActionExecuting(filterContext);
        }

    }
}