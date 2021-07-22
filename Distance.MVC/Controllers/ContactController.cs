using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Distance.Business;
using Distance.Business.Entitiy;
using Distance.Business.Interfaces;
using Distance.EntityFrame;
using Distance.EntityFrame.Repository;
using Distance.MVC.Helpers;
using Omu.ValueInjecter;
using Distance.GoogleConsumer;
using System.Configuration;
using System.Net;
using System.Threading;

namespace Distance.MVC.Controllers
{
    public class ContactController : MyController
    {

        public ActionResult Index()
        {
            if (ConfigurationManager.AppSettings["DontCheckFirewallAtHome"] != "true")
                try
                {
                    Geocoder.GetLocation("London");
                }
                catch (Exception ex)
                {
                    ViewBag.FirewallException = ex.Message;
                }

            var list = _contactRepository
                            .All
                            .ToList();

            return View(list );
        }

        public ActionResult Edit(int id = 0)
        {
            var contact = _contactRepository.Get(id) ?? new Contact();
            contact.DistanceComparisons = _discoRepository.Find(x => x.Customer.ContactId == id).ToList(); 

            ViewBag.marker = "Content/" + ( contact.ExistingMarker() ?? "Markers/dots/red-dot.png" );

            return View( contact );
        }

        [HttpPost]
        public ActionResult Edit(Contact contact)
        {
            if (contact.ContactId != 0)
                _contactRepository.Update(contact);
            else 
                _contactRepository.Save(contact);
                
            var postcodes = Request["postcodes"].Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            contact.Addresses = _addressRepository.Find(a => a.Contact.ContactId == contact.ContactId).ToList(); 
            contact.UpdateAddressesByPostcodes(
                postcodes, 
                Request["CountryValidator"], 
                _contactRepository, 
                _addressRepository );

            foreach (var address in contact.Addresses.Where(a => a.Lat == null))
            {
                try
                {
                    var location = Geocoder.GetLocation(address.PostCode);
                    address.Lat = location.Lat;
                    address.Lng = location.Lng;
                    _addressRepository.Save(address);
                }
                catch 
                {
                    if (contact.InvalidPostCodes == null)  
                        contact.InvalidPostCodes = new List<string>();
                    contact.InvalidPostCodes.Add( address.PostCode );
                }
                Thread.Sleep(100);
            }

            if (contact.ContactType == ContactType.Enterprise)
            {
                var context = _scope.Resolve<EfUnitOfWork>();
                context.Configuration.ProxyCreationEnabled = false;
                context.Configuration.LazyLoadingEnabled = false;

                contact = _contactRepository.Find(x => x.ContactId == contact.ContactId)
                    .Include(x => x.Addresses).FirstOrDefault();

                Statics.UpdateEnterpriseRootContact(contact);
            }

            if (contact.InvalidPostCodes == null)
                return RedirectToAction("Edit", new { id = contact.ContactId }); 
            else
                return View( contact );

        }

        public ActionResult Delete(int id)
        {
            var contact = _contactRepository.Get(id) ;
            if (contact == null)
                throw new NullReferenceException("No such contact!");

            contact.Delete( _contactRepository, _addressRepository, _discoRepository, _reportRepository ); 
                
            return RedirectToAction("Index");
        }


        public ActionResult Locations(int id)
        {
//            if (!Request.IsAjaxRequest()) return new HttpStatusCodeResult(HttpStatusCode.BadRequest );
            var contact = _contactRepository.Get(id);

            var locations = contact.Addresses
                .Where(x => x.Lat != null)
                .ToList()
                .Select(x => new Address() { PostCode= x.PostCode, Lat = x.Lat, Lng = x.Lng })
                .ToList();

            return Json(locations, JsonRequestBehavior.AllowGet);
        }

    }
}
