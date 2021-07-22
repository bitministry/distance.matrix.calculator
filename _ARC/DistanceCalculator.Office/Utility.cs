using DistanceCalculatorUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistanceCalculator.Office
{
    public class Utility
    {
        [ComVisible(true)]
        public List<string> CalculatePostcodes(List<string> custPostcodes, List<string> targetPostcodes)
        {
            List<string> output = new List<string>();

            DateTime startTime = DateTime.Now;

            Hashtable targets = new Hashtable();
            Hashtable targetHeaders = new Hashtable();

            var custName = string.Empty;

            var headerLine = string.Empty;

            var googleApiKey = ConfigurationManager.AppSettings["googleApiKey"].ToString();
            var cc = new CalculateClass(googleApiKey);

            for (int i = 1; i <= targets.Count; i++)
            {
                cc.GetDistanceMatrix(custPostcodes, targets[targetHeaders[i]] as List<string>, targetHeaders[i] as string);
            }

            //write a header line to indicate company names
            StringBuilder compNames = new StringBuilder();
            StringBuilder headers = new StringBuilder();
            compNames.AppendFormat("{0},", custName);
            headers.Append("Start Postcode,Start Address");
            for (int i = 1; i <= targetHeaders.Count; i++)
            {
                compNames.AppendFormat(",{0},,,", targetHeaders[i]);
                headers.Append(",End Postcode,End Address,Distance (miles),Distance (km)");
            }
            output.Add(compNames.ToString());
            output.Add(headers.ToString());
            foreach (PostcodePair item in cc.Results.Values)
            {
                output.Add(item.ToString());
            }

            DateTime endTime = DateTime.Now;

            return output;
        }

        // Returns an echo of the input string.  Used for testing
        [ComVisible(true)]
        public string Test(string inputString)
        {
            return string.Format("echo {0}", inputString);
        }
    }
}
