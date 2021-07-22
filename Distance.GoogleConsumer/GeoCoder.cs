using Distance.Business.Entitiy;
using GoogleMapsApi.Entities.Geocoding.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Distance.GoogleConsumer
{
    public static class Geocoder
    {
        public static string GoogleApiKey;
        public static int WaitAfterRequest; 

        public static GeoLocation GetLocation(string address)
        {
            var fullUrl = String.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}",
                address, GoogleApiKey);

            var geocodeRequest = new Uri(fullUrl);
            var webClient = new WebClient();

            var responseContent = webClient.OpenRead(geocodeRequest);
            var dataContractJsonSerializer = new DataContractJsonSerializer(typeof(GeocodingResponse));

            if (responseContent != null)
            {
                var response = dataContractJsonSerializer.ReadObject(responseContent) as GeocodingResponse;
                var firstResult = response.Results.FirstOrDefault();

                if (firstResult != null )
                    return new GeoLocation() {
                        Lat = firstResult.Geometry.Location.Latitude ,
                        Lng = firstResult.Geometry.Location.Longitude 
                    };
                throw new Exception("Google did not return GeoLocation for " + address);
            }

            throw new Exception("Google returned null for "+ address);

        }

    }
}
