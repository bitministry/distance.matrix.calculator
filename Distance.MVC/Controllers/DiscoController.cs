using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using Autofac;
using Distance.Business.Entitiy;
using Distance.Business.Helpers;
using Distance.Business.Interfaces;
using Distance.EntityFrame;
using Distance.GoogleConsumer;
using Distance.MVC.Helpers;
using WebGrease.Css.Extensions;
using System.Web.Script.Serialization;
using Distance.Business;

namespace Distance.MVC.Controllers
{

    public class DiscoController : MyController
    {
        public static string _reportsDumpDirectory; 
        public DiscoController()
        {
            _reportsDumpDirectory = _reportsDumpDirectory ?? 
                ( String.IsNullOrEmpty(ConfigurationManager.AppSettings["ReportsDumpDirectory"])
                     ? new DirectoryInfo(HostingEnvironment.ApplicationPhysicalPath).Parent.FullName + @"\DiscoDump\" 
                     : ConfigurationManager.AppSettings["ReportsDumpDirectory"] );

            if (! Directory.Exists(_reportsDumpDirectory)) Directory.CreateDirectory(_reportsDumpDirectory); 
        }

        public ActionResult DeleteReport(int id)
        {
            var report = _reportRepository.Get(id);
            var discoId = report.DistanceComparison.DistanceComparisonId;
            _reportRepository.Delete( report  );
            return RedirectToAction("DetailsForId", new { id = discoId}); 
        }

        public ActionResult Details(string competitorIds, int customerId )
        {
            var arrCompetitorIds = competitorIds.Split(',').Where( x=> x.IsNumeric() )
                .Select( int.Parse ).ToArray();

            var disco = _discoRepository.Find(x => x.Customer.ContactId == customerId
                    && x.CompetitorsIncluded.All(y => arrCompetitorIds.Contains(y.ContactId))
                    && x.CompetitorsIncluded.Count == arrCompetitorIds.Count()).FirstOrDefault();

            if (disco == null)
            {
                disco = new DistanceComparison()
                {
                    Customer = _contactRepository.Get(customerId),
                    CompetitorsIncluded = new List<Contact>()
                };
                arrCompetitorIds.ForEach( c=> disco.CompetitorsIncluded.Add( _contactRepository.Get(c ) ) );
                _discoRepository.Save(disco);
            }

            return View( disco );
        }

        public ActionResult DetailsForId( int id )
        {
            var disco = _discoRepository.Get(id);
            return View( "Details", disco );
        }


        public ActionResult Report(int id)
        {
            var report = _reportRepository.Get(id);

            report.CustomerWithDistances = BinaryDeserializer<Contact>.Deserialize(
                report.Serialized_Contact_Customer 
                ?? System.IO.File.ReadAllBytes(_reportsDumpDirectory + @"\" + id));

            if (report.Serialized_ICollectionFetcherLogForTarget != null )
                report.Log = BinaryDeserializer<ICollection<ComparisonReport.LogForTarget>>.Deserialize(report.Serialized_ICollectionFetcherLogForTarget);


            var xiCompetitorsWithoutMarkerCount = -1;
            var markerColors = new string[] {"pink", "orange", "purple", "blue", "green", "red"};
            var compeditorMarkers = new Dictionary<int, string>();
            
            foreach (var contact in report.DistanceComparison.CompetitorsIncluded)
                compeditorMarkers[contact.ContactId] = contact.ExistingMarker() ?? ((Func<string>)(() =>
                {
                    xiCompetitorsWithoutMarkerCount++;
                    return String.Format("Markers/dots/{0}-dot.png", markerColors[xiCompetitorsWithoutMarkerCount]);
                }))(); 

            var jsonLinq = report.CustomerWithDistances.Addresses.Where( a=> a.SpansToTargets != null ).Select(a => new
            {
                CustomerAddressId = a.AddressId,
                CustomerAddressLat = a.Lat,
                CustomerAddressLng = a.Lng,
                CustomerAddressPostCode = a.PostCode,
                SpansToEnterprise = a.SpansToTargets.FirstOrDefault(x => x.Destination.ContactId == Statics.EnterpriseRootContact.ContactId)
                    .Spans.OrderByDescending(x => x.Error == null).ThenBy(x => x.Meter).Take(5)
                    .Select( es => new { 
                        es.Destination.AddressId,
                        es.Destination.PostCode,
                        es.Destination.Lat,
                        es.Destination.Lng,
                        Distance = es.Meter.MetersToMiles(),
                        es.Error
                    } ),
                SpansToCompetitors = a.SpansToTargets.Where(x => x.Destination.ContactId != Statics.EnterpriseRootContact.ContactId)
                    .Select(cs => new
                    {
                        CompetitorContactId = cs.Destination.ContactId,
                        Icon = compeditorMarkers[cs.Destination.ContactId],
                        CompetitorName = cs.Destination.Name,
                        SpansTo = cs.Spans.OrderByDescending(x => x.Error == null).ThenBy(x => x.Meter).Take(5)
                            .Select(x => new
                            {
                                x.Destination.AddressId,
                                x.Destination.PostCode,
                                x.Destination.Lat,
                                x.Destination.Lng,
                                Distance = x.Meter.MetersToMiles(),
                                x.Error
                            })
                    })
            });
            ViewBag.json = new JavaScriptSerializer().Serialize(jsonLinq);


            var averageDistances = new Dictionary<int, int?>();

            averageDistances.Add( 
                Statics.EnterpriseRootContact.ContactId, 
                (int?)report.CustomerWithDistances.Addresses.Where(x => x.SpansToTargets != null).SelectMany(
                      x => x.SpansToTargets.Where(y => y.Destination.ContactId == Statics.EnterpriseRootContact.ContactId))
                      .Select(x => x.Spans.Min(y => y.Meter)).Average() );
            
            foreach (var competitor in report.DistanceComparison.CompetitorsIncluded)
                averageDistances.Add( 
                    competitor.ContactId, 
                    (int?) report.CustomerWithDistances.Addresses.Where(x => x.SpansToTargets != null).SelectMany(
                        x => x.SpansToTargets.Where(y => y.Destination.ContactId == competitor.ContactId))
                        .Select(x => x.Spans.Min(y => y.Meter)).Average());

            ViewBag.averageDistances = averageDistances;
            ViewBag.minimumDistance = averageDistances.Min(x => x.Value);

            return View( report );
        }

        public ActionResult NewReportForDisco( int id  )
        {
            var context = _scope.Resolve<EfUnitOfWork>();
            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.LazyLoadingEnabled = false;

            var disco = _discoRepository.Find(x => x.DistanceComparisonId == id)
                .Include(x => x.Customer)
                .Include(x => x.CompetitorsIncluded)
                .FirstOrDefault();

            disco.Customer = _contactRepository.Find(x => x.ContactId == disco.Customer.ContactId)
                .Include(x => x.Addresses).FirstOrDefault();

            disco.CompetitorsIncluded = disco.CompetitorsIncluded.Select(q => _contactRepository.Find(x => x.ContactId == q.ContactId)
                    .Include(x => x.Addresses).FirstOrDefault()).ToList();


            var fetcher = new ComparisonReportFetcher(
                reportsDumpDirectory : _reportsDumpDirectory, 
                discoWithoutProxies: disco,
                contactRepo: _contactRepository,
                reportRepo: _reportRepository,
                waitAfterRequest: int.Parse(ConfigurationManager.AppSettings["Google.Distance.WaitAfterRequest"]),
                dimensionSize: int.Parse(ConfigurationManager.AppSettings["Google.Distance.DimensionSize"])
                );

            new Thread(() => fetcher.Fetch()).Start();

            return Json(new { ComparisonReportId = fetcher.ComparisonReportId }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProgressStatusForReport( int id )
        {

            var report = _reportRepository.Get(id);

            var result = new {
                report.NoReplyError, 
                PercentageCompleted = report.PercentageCompleted()
            };

            return Json(result , JsonRequestBehavior.AllowGet);
        }


    }
}