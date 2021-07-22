using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Distance.Business.Entitiy;
using Distance.Business.Interfaces;

namespace Distance.Business
{
    public static class Statics
    {
//        public static LifetimeScope AutofacContainer;

        public static CultureInfo CurrentCulture; 

        public static long SpanValidityInMinutes = 1440;
        
        public static int DistanceMatrixDimensionSize = 10; 

        #region PostCode validation 

        private static class CountryValidators
        {
            public static readonly Regex Any = new Regex("", RegexOptions.Compiled);
            public static readonly Regex AUT = new Regex("[0-9]{4,4}");
            public static readonly Regex AUS = new Regex("[2-9][0-9]{2,3}");
            public static readonly Regex BRA = new Regex("([0-9]{1})([0-9]{1})([0-9]{1})([0-9]{1})([0-9]{1})-?([0-9]{3})");
            public static readonly Regex CAN = new Regex(@"[a-zA-Z].[0-9].[a-zA-Z].\s[0-9].[a-zA-Z].[0-9].");
            public static readonly Regex EST = new Regex("[0-9]{5,5}");
            public static readonly Regex GER = new Regex("[0-9]{5,5}");
            public static readonly Regex ITA = new Regex("[0-9]{5,5}");
            public static readonly Regex NLD = new Regex(@"[0-9]{4,4}\s[a-zA-Z]{2,2}");
            public static readonly Regex PRT = new Regex("[0-9]{4,4}-[0-9]{3,3}");
            public static readonly Regex SWE = new Regex(@"[0-9]{3,3}\s[0-9]{2,2}");
            public static readonly Regex GBR = new Regex(@"(GIR 0AA)|((([A-Z-[QVX]][0-9][0-9]?)|(([A-Z-[QVX]][A-Z-[IJZ]][0-9][0-9]?)|(([A-Z-[QVX]][0-9][A-HJKSTUW])|([A-Z-[QVX]][A-Z-[IJZ]][0-9][ABEHMNPRVWXY])))) [0-9][A-Z-[CIKMOV]]{2})");
            public static readonly Regex USA = new Regex(@"[0-9]{5,5}[\-]{0,1}[0-9]{4,4}");
        }

        public static string[] CountriesWithValidators()
        {
            var stats = typeof(CountryValidators).GetFields();
            return stats.Select(stat => stat.Name).ToArray();
        }


        public static Regex CountryPostCodeValidator(string isoCountry3)
        {
            switch (isoCountry3)
            {
                case "AUT": return CountryValidators.AUT;
                case "AUS": return CountryValidators.AUS;
                case "BRA": return CountryValidators.BRA;
                case "CAN": return CountryValidators.CAN;
                case "EST": return CountryValidators.EST;
                case "GER": return CountryValidators.GER;
                case "ITA": return CountryValidators.ITA;
                case "NLD": return CountryValidators.NLD;
                case "PRT": return CountryValidators.PRT;
                case "SWE": return CountryValidators.SWE;
                case "GBR": return CountryValidators.GBR;
                case "USA": return CountryValidators.USA;
                default: return CountryValidators.Any;
            }
        }

        #endregion

        private static Contact _enterprise;

        public static Contact EnterpriseRootContact
        {
            get
            {
                if (_enterprise == null)
                    throw new NullReferenceException("EnterpriseRootContact not initialized. Use EnterpriseInitialize( contactRepository )!");
                return _enterprise;
            }
        }

        public static void UpdateEnterpriseRootContact( Contact enterprice )
        {
            _enterprise = enterprice; 
        }
    }
}
