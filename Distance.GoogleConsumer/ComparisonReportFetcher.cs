using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading;
using Distance.Business.Entitiy;
using Distance.Business.Interfaces;
using GoogleMaps.JSON;
using Distance.Business;
using Distance.Business.Helpers;


namespace Distance.GoogleConsumer
{
    public class ComparisonReportFetcher
    {

        private readonly ComparisonReport _report;

        private readonly string _baseUrl;
        private readonly string _baseQueryStringPattern;
        private readonly int _dimensionSize;
        private readonly int _waitAfterRequest;
        private readonly string _reportsDumpDirectory;

        private readonly IGenericRepository<Contact> _contactRepo;
        private readonly IGenericRepository<ComparisonReport> _reportRepo;

        private string _lastRequestQuerystring;

        static int AvailableGoogleApiKeyCount;

        public ComparisonReportFetcher(
            string reportsDumpDirectory,
            DistanceComparison discoWithoutProxies,
            IGenericRepository<Contact> contactRepo,
            IGenericRepository<ComparisonReport> reportRepo,
            int dimensionSize = 10,
            int waitAfterRequest = 1000,
            string baseUrl = "https://maps.googleapis.com/maps/api/distancematrix/json?",
            string baseQueryStringPattern = "origins={0}&destinations={1}&sensor=false&key={2}")
        {
            int.TryParse(ConfigurationManager.AppSettings["Google.maps.distance.apiKey.count"], out AvailableGoogleApiKeyCount);

            GoogleApiKey = ConfigurationManager.AppSettings["Google.maps.distance.apiKey." + RankOfTheGoogleApiKeyInUse];
            if (String.IsNullOrEmpty(GoogleApiKey)) throw new NullReferenceException("GoogleApiKey.Value");

            if (contactRepo == null) throw new ArgumentNullException("contactRepo");
            if (reportRepo == null) throw new ArgumentNullException("reportRepo");
            if (discoWithoutProxies == null) throw new ArgumentNullException("discoWithoutProxies");
            if (String.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException("baseUrl");
            if (String.IsNullOrEmpty(baseQueryStringPattern)) throw new ArgumentNullException("baseQueryStringPattern");
            if (String.IsNullOrEmpty(reportsDumpDirectory)) throw new ArgumentNullException("reportsDumpDirectory");
            _baseUrl = baseUrl;
            _dimensionSize = dimensionSize;
            _waitAfterRequest = waitAfterRequest;
            _baseQueryStringPattern = baseQueryStringPattern;
            _reportsDumpDirectory = reportsDumpDirectory;
            _contactRepo = contactRepo;
            _reportRepo = reportRepo;

            _report = new ComparisonReport()
            {
                DistanceComparison = discoWithoutProxies,
                CustomerWithDistances = discoWithoutProxies.Customer,
                Log = new Collection<ComparisonReport.LogForTarget>(),
                Created = DateTime.Now
            };
            _reportRepo.Save(_report);
            // sort postcodes
            _report.CustomerWithDistances.Addresses = _report.CustomerWithDistances.Addresses.OrderBy(x => x.PostCode).ToList();
        }

        public int ComparisonReportId
        {
            get { return _report.ComparisonReportId; }
        }

        private Dictionary<int, int> _googleExceptionCounts;
        private enum GoogleRequestPipe { BeforeRequest, GotResponse, ResponseNotNull }

        public ComparisonReport Fetch()
        {
            _googleExceptionCounts = new Dictionary<int, int>();
            _googleExceptionCounts.Add((int)GoogleRequestPipe.BeforeRequest, 0);
            _googleExceptionCounts.Add((int)GoogleRequestPipe.GotResponse, 0);
            _googleExceptionCounts.Add((int)GoogleRequestPipe.ResponseNotNull, 0);

            GetDistancesFor(Statics.EnterpriseRootContact);

            foreach (var compeditor in _report.DistanceComparison.CompetitorsIncluded)
                GetDistancesFor(compeditor);

            SaveReport();

            return _report;
        }

        private void SaveReport()
        {
            _report.Serialized_ICollectionFetcherLogForTarget = BinarySerializer.Serialize(_report.Log);
            _reportRepo.Save(_report);

            var serialized_Contact_Customer = BinarySerializer.Serialize(_report.CustomerWithDistances);
            try
            {
                _report.Serialized_Contact_Customer = serialized_Contact_Customer;
                _reportRepo.Save(_report);
            }
            catch (Exception ex)
            {
                File.WriteAllBytes(_reportsDumpDirectory + @"\" + _report.ComparisonReportId,
                    BinarySerializer.Serialize(_report.CustomerWithDistances));
            }
        }


        private ComparisonReport.LogForTarget _currentLogForTarget;

        void GetDistancesFor(Contact targetContact)
        {
            _currentLogForTarget = new ComparisonReport.LogForTarget(targetContact);
            _report.Log.Add(_currentLogForTarget);

            if (_report.NoReplyError != null) return;

            var originPage = 0;
            var originTotal = _report.CustomerWithDistances.Addresses.Count;
            var targetTotal = targetContact.Addresses.Count;

            //while we still have postcodes to iterate in the customer collection
            while (((originPage) * _dimensionSize) < originTotal)
            {
                var originSubSet = _report.CustomerWithDistances.Addresses
                    .Skip(originPage * _dimensionSize).Take(_dimensionSize).ToList();

                var targetPage = 0;

                //while we still have postcodes to iterate in the targetContact collection
                while (((targetPage) * _dimensionSize) < targetTotal)
                {
                    var targetSubSet = targetContact.Addresses
                        .Skip(targetPage * _dimensionSize).Take(_dimensionSize).ToList();

                    GoogleResponse distanceMatrix = null;
                    var looper = 0;

                    while (distanceMatrix == null && looper < 6)
                    {
                        distanceMatrix = SendRequest(originSubSet, targetSubSet);
                        if (_report.NoReplyError != null) return;
                        looper++;
                        Thread.Sleep(_waitAfterRequest);
                    }
                    if (distanceMatrix == null)
                    {
                        _report.NoReplyError = "Run out of keys: " + AvailableGoogleApiKeyCount;
                        SaveReport();
                        return;
                    }

                    FillOriginAddressesTargetDestinationsSpans(targetContact, distanceMatrix, originSubSet, targetSubSet);

                    targetPage++;

                    _report.ProcessedElements += originSubSet.Count * targetSubSet.Count;
                    _reportRepo.Save(_report);
                }
                originPage++;
            }

        }

        private void FillOriginAddressesTargetDestinationsSpans(Contact targetContact, GoogleResponse distanceMatrix, List<Address> originSubSet, List<Address> targetSubSet)
        {
            for (int rowcount = 0; rowcount < originSubSet.Count; rowcount++)
            {
                var customerAddress = originSubSet[rowcount];
                if (distanceMatrix != null && distanceMatrix.origin_addresses.Length == originSubSet.Count)
                    customerAddress.Line1 = distanceMatrix.origin_addresses[rowcount];

                if (customerAddress.SpansToTargets == null)
                    customerAddress.SpansToTargets = new List<Address.SpansToContact>();

                var spansToTarget = customerAddress.SpansToTargets.FirstOrDefault(x => x.Destination.ContactId == targetContact.ContactId);
                if (spansToTarget.Destination == null)
                {
                    spansToTarget.Destination = targetContact;
                    spansToTarget.Spans = new Collection<Span>();
                    customerAddress.SpansToTargets.Add(spansToTarget);
                }

                for (int colcount = 0; colcount < targetSubSet.Count; colcount++)
                {
                    var targetAddress = targetSubSet[colcount];

                    var span = new Span()
                    {
                        Origin = customerAddress,
                        Destination = targetAddress,
                        Created = DateTime.Now
                    };
                    spansToTarget.Spans.Add(span);

                    if (distanceMatrix != null && distanceMatrix.origin_addresses.Length == originSubSet.Count)
                    {
                        if (distanceMatrix.rows[rowcount].elements[colcount].status == "OK")
                        {
                            span.Meter = distanceMatrix.rows[rowcount].elements[colcount].distance.value;
                            span.Destination.Line1 = distanceMatrix.destination_addresses[colcount];
                        }
                        else
                            span.Error = distanceMatrix.rows[rowcount].elements[colcount].status;
                    }
                    else
                        span.Error = "no response";
                }
            }
        }

        static string GoogleApiKey;
        static int RankOfTheGoogleApiKeyInUse = 0;

        private GoogleResponse SendRequest(IEnumerable<Address> custSubSet, IEnumerable<Address> targetSubSet)
        {
            //generate the query string
            _lastRequestQuerystring = string.Format(_baseQueryStringPattern,
                string.Join("|", custSubSet.Select(a => a.PostCode)),
                string.Join("|", targetSubSet.Select(a => a.PostCode)), GoogleApiKey);
            var fullUrl = _baseUrl + _lastRequestQuerystring;

            var geocodeRequest = new Uri(fullUrl);
            var webClient = new WebClient();
            GoogleRequestPipe googleRequestPipeStep = 0; // BeforeRequest
            try
            {
                var responseContent = webClient.OpenRead(geocodeRequest);
                googleRequestPipeStep++; // GotResponse

                if (responseContent == null)
                    throw new Exception("Google returned null");
                googleRequestPipeStep++; // ResponseNotNull

                var dataContractJsonSerializer = new DataContractJsonSerializer(typeof(GoogleResponse));
                var googleResponse = dataContractJsonSerializer.ReadObject(responseContent) as GoogleResponse;
                if (googleResponse == null)
                    throw new Exception("Cant serialize response");
                googleRequestPipeStep++; // JsonDeserialized

                switch (googleResponse.status)
                {
                    case "OK":
                        return googleResponse;
                    case "REQUEST_DENIED":
                        _report.NoReplyError = googleResponse.status;
                        SaveReport();
                        break;
                    case "OVER_QUERY_LIMIT":
                        // switch keys

                        if (RankOfTheGoogleApiKeyInUse < AvailableGoogleApiKeyCount)
                            RankOfTheGoogleApiKeyInUse++;
                        else
                        {
                            RankOfTheGoogleApiKeyInUse = 0;
                        }
                        GoogleApiKey = ConfigurationManager.AppSettings["Google.maps.distance.apiKey." + RankOfTheGoogleApiKeyInUse];
                        return null;
                }
                throw new Exception(String.Format("google returned: {0}", googleResponse.status));
            }
            catch (Exception ex)
            {
                _currentLogForTarget.RequestFailureLog.Add(
                    new ComparisonReport.LogForTarget.RequestFailureLogItem(
                        queryString: _lastRequestQuerystring,
                        exceptionMessage: ex.Message));

                // group and exceptions by stage in pipe
                _googleExceptionCounts[(int)googleRequestPipeStep]++;
                // stop the generation when the exceptions pile up before Json is successfully deserialized
                if (_googleExceptionCounts[(int)GoogleRequestPipe.BeforeRequest] == 4)
                    _report.NoReplyError = "Can't send requests, times: " + 4;

                if (_googleExceptionCounts[(int)GoogleRequestPipe.GotResponse] == 8)
                    _report.NoReplyError = "Google is returning null, times: " + 8;

                if (_googleExceptionCounts[(int)GoogleRequestPipe.ResponseNotNull] == 16)
                    _report.NoReplyError = "Cant understand google's response, times: " + 16;

                return null;
            }
        }




        #region GoogleMock

        public static GoogleResponse GoogleMock(IList<Address> origins, IList<Address> targets)
        {
            if (origins.Count < 5 || targets.Count < 5)
                return null;

            var resp = new GoogleResponse();

            resp.origin_addresses = origins.Select(x => x.PostCode).ToArray();
            resp.destination_addresses = targets.Select(x => x.PostCode).ToArray();

            var rows = new List<Rows>();
            foreach (var origin in origins)
            {
                var row = new Rows();
                var elements = new List<Elements>();
                var looper = 0;
                foreach (var target in targets)
                {
                    var el = new Elements();
                    el.distance = new GoogleMaps.JSON.Distance() { value = 123 };

                    el.status = looper != 7 ? "OK" : "NOT OK";

                    elements.Add(el);
                    looper++;
                }
                row.elements = elements.ToArray();
                rows.Add(row);
            }
            resp.rows = rows.ToArray();

            return resp;
        }
        #endregion

    }
}

