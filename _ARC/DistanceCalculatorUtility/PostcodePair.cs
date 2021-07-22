using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace DistanceCalculatorUtility
{
    public class PostcodePair
    {
        public string StartPostcode { get; set; }
        public string StartLocation { get; set; }
        public Hashtable EndLocations { get; set; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (EndLocation item in EndLocations.Values)
            {
                sb.AppendFormat(",{0},\"{1}\",{2}, {3}", item.EndPostcode, item.EndLoc, Math.Round(item.Distance / 1609.344, 2), Math.Round(item.Distance / 1000, 2));
            }
            return string.Format("{0},\"{1}\"{2}", StartPostcode, StartLocation, sb.ToString());
        }
    }

    public class EndLocation
    {
        public string Supplier {get;set;}
        public string EndPostcode { get; set; }
        public string EndLoc { get; set; }
        public double Distance { get; set; }

    }
}
