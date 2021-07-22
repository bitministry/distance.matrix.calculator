using System;
using System.Web;
using System.Web.Mvc;
using Distance.Business.Entitiy;

namespace Distance.MVC.Helpers
{
    public static class MyExtensions 
    {
        public static string MetersToMiles(this int? meters)
        {
            if (meters == null) return null;
            return Math.Round((double) meters/1609.344, 2) + " mi";
        }
        public static bool IsNumeric(this string s)
        {
            float output;
            return float.TryParse(s, out output);
        }
        public static string ExistingMarker(this Contact c)
        {
            string xxs = String.Format("Markers/{0}.png", c.Name);
            return System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/" + xxs)) ? xxs : null; 
        }
    }

}