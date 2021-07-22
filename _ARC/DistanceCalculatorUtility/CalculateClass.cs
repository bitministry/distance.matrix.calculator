using BingMapsRESTService.Common.JSON;
using GoogleMaps.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistanceCalculatorUtility
{
    public class CalculateClass
    {
        string _key = string.Empty;

        Hashtable _results = new Hashtable();

        public CalculateClass()
        {
            _key = "AnQtRTA47D0MTu-BJKTzoMddX38Yx5JvxykNry2N_pQ6sLPA04fm3FDxJ0iO1Qmf";
        }

        public CalculateClass(string key)
        {
            _key = key;
        }


        public Hashtable Results { get { return _results; } }

        #region Google api

        public void GetDistanceMatrix(List<string> custPostcodes, List<string> targetPostcodes, string supplierName)
        {
            int custPage = 0;
            int custTotal = custPostcodes.Count;
            int targetTotal = targetPostcodes.Count;
            var recordCount = 0;
            int custPageSize = 10;
            int targetPageSize = 10;
            var baseUrl = "https://maps.googleapis.com/maps/api/distancematrix/json?";

            // generate Google api queries in batches of <100.  Pause for 10 seconds and send another request

            //while we still have postcodes to iterate in the customer collection
            while (((custPage) * custPageSize) < custTotal)
            {
                var custSubSet = custPostcodes.Skip(custPage * custPageSize).Take(custPageSize).ToList<string>();

                int targetPage = 0;

                //while we still have postcodes to iterate in the target collection
                while (((targetPage) * targetPageSize) < targetTotal)
                {
                    var targetSubSet = targetPostcodes.Skip(targetPage * targetPageSize).Take(targetPageSize).ToList<string>();

                    //generate the query string
                    var querystring = string.Format("origins={0}&destinations={1}&sensor=false&key={2}",string.Join("|", custSubSet),string.Join("|", targetSubSet),_key);
                    var fullUrl = baseUrl + querystring;

                    Uri geocodeRequest = new Uri(fullUrl);
                    WebClient wc = new WebClient();
                    System.IO.Stream content = null;
                    try
                    {
                        content = wc.OpenRead(geocodeRequest);
                    }
                    catch (WebException ex)
                    {
                        throw;
                    }
                    catch(Exception)
                    {

                    }

                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(GoogleResponse));
                    var x = (ser.ReadObject(content) as GoogleResponse);


                    for (int rowcount = 0; rowcount < x.rows.Length; rowcount++)
                    {
                        for (int colcount = 0; colcount < x.rows[rowcount].elements.Length; colcount++)
                        {
                            recordCount++;
                            if (x.rows[rowcount].elements[colcount].status == "OK")
                            {
                                PostcodePair pcp = _results[custSubSet[rowcount]] as PostcodePair;
                                if (pcp == null)
                                {
                                    // no records for this customer postcode, so create new one
                                    pcp = new PostcodePair
                                    {
                                        StartPostcode = custSubSet[rowcount],
                                        StartLocation = x.origin_addresses[rowcount]
                                    };
                                    // now create the first supplier for the postcode pair
                                    pcp.EndLocations = new Hashtable();
                                    pcp.EndLocations[supplierName] = new EndLocation
                                    {
                                        Supplier = supplierName,
                                        EndPostcode = targetSubSet[colcount],
                                        EndLoc = x.destination_addresses[colcount],
                                        Distance = x.rows[rowcount].elements[colcount].distance.value
                                    };
                                    _results[custSubSet[rowcount]] = pcp;
                                }
                                else
                                {
                                    // we already have a record, so need to check end distance for this supplier. 
                                    // first check if there is already a record for this supplier
                                    EndLocation suppLoc = pcp.EndLocations[supplierName] as EndLocation;
                                    if (suppLoc == null)
                                    {
                                        suppLoc = new EndLocation
                                        {
                                            Supplier = supplierName,
                                            EndPostcode = targetSubSet[colcount],
                                            EndLoc = x.destination_addresses[colcount],
                                            Distance = x.rows[rowcount].elements[colcount].distance.value
                                        };
                                        pcp.EndLocations[supplierName] = suppLoc;
                                    }
                                    else
                                    {
                                        // If this is closer, replace the existing one
                                        if (suppLoc.Distance > x.rows[rowcount].elements[colcount].distance.value)
                                        {
                                            suppLoc.EndPostcode = targetSubSet[colcount];
                                            suppLoc.EndLoc = x.destination_addresses[colcount];
                                            suppLoc.Distance = x.rows[rowcount].elements[colcount].distance.value;
                                        }
                                    }
                                }
                            }
                            Console.WriteLine(string.Format("{0} calculations of {1} complete", recordCount, (targetTotal * custTotal)));
                        }
                    }

                    // wait for 10 seconds so we can send another request to google (what a stupid restriction!)
                    //Thread.Sleep(1000);
                    targetPage++;
                }

                custPage++;
            }

        }


        #endregion 
    }
}
