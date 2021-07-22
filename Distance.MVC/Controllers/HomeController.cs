using Distance.GoogleConsumer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Distance.MVC.Controllers
{
    public class HomeController : Controller
    {
        // lalala

        public ActionResult Logout()
        {

            Session["IsAuthorized"] = null;
            return Login();
        }

        public ActionResult Login()
        {
            if (Request.Form["password"] == ConfigurationManager.AppSettings["LoginPassword"])
            {
                Session["IsAuthorized"] = true;
                return RedirectToAction("Index", "Contact");
            }

            return View("Login");
        }

        public ActionResult TestView(string id)
        {
            return View( id );
        }

    }
}