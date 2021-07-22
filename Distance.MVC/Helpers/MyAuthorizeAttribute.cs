using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distance.MVC.Helpers
{
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["IsAuthorized"] == null)
            {
                if (ConfigurationManager.AppSettings["AutoLogin"] == "true")
                    filterContext.HttpContext.Session["IsAuthorized"] = true;
                else
                    filterContext.Result = new RedirectResult("~/Home/Login");
            }
            
        }
    }
}