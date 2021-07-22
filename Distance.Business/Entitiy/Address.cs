using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Distance.Business.Entitiy
{

    [Serializable]
    public class Address : GeoLocation 
    {
        public int AddressId { get; set; }

        public virtual Contact Contact { get; set; }

        public Address(){ }

        public Address(string countryCodeIso3, string postCode )
        {
            if (countryCodeIso3.Length != 3) 
                throw new InvalidDataException("CountryCodeIso3 not valid!");

            if ( ! Statics.CountryPostCodeValidator(countryCodeIso3).IsMatch(postCode)
                   ||
                   String.IsNullOrEmpty(postCode))
                throw new InvalidDataException("Postcode not valid!");

            CountryCodeIso3 = countryCodeIso3;
            PostCode = postCode;
        }

        public string PostCode { get; set; }
        public string CountryCodeIso3 { get; set; }

        public string Line1 { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }


        public IList<SpansToContact> SpansToTargets;

        [Serializable]
        public struct SpansToContact
        {
            public Contact Destination { get; set; }
            public ICollection<Span> Spans { get; set; }
        }

//        public DateTime LastDestinationAdded { get; set; }

    }
}
