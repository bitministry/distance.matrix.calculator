using DistanceCalculatorUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace DistanceCalculator
{
    class Program
    {


        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;

            string fileName = string.Empty;
            var writeFile = string.Empty;

            if (!string.IsNullOrEmpty(args[0]))
            {
                fileName = args[0];
            }
            else return;
            if (!string.IsNullOrEmpty(args[1]))
            {
                writeFile = args[1];
            }
            else return;

            var reader = new StreamReader(File.OpenRead(fileName));
            Hashtable targets = new Hashtable();
            Hashtable targetHeaders = new Hashtable();
            List<string> custPostcodes = new List<string>();
            List<string> targetPostcodes = new List<string>();
            var custName = string.Empty;

            var importLineCounter = 0;
            var headerLine = string.Empty;

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                //First line of file to contain headers and ability to deal with multiple competitor columns
                if (importLineCounter == 0)
                {
                    headerLine = line;
                    custName = values[0];
                    for (int i = 1; i < values.Length; i++)
                    {
                        targets.Add(values[i], new List<string>());
                        targetHeaders.Add(i, values[i]);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(values[0]))
                    {
                        custPostcodes.Add(values[0]);
                    }
                    for (int i = 1; i < values.Length; i++)
                    {
                        targetPostcodes = null;
                        if (!string.IsNullOrEmpty(values[i]))
                        {
                            targetPostcodes = targets[targetHeaders[i]] as List<string>;
                            targetPostcodes.Add(values[i]);
                        }
                    }
                }
                importLineCounter++;
            }

            reader.Close();
            reader.Dispose();
            var googleApiKey = ConfigurationManager.AppSettings["googleApiKey"].ToString();
            var cc = new CalculateClass(googleApiKey);

            for (int i = 1; i <= targets.Count; i++)
            {
                cc.GetDistanceMatrix(custPostcodes, targets[targetHeaders[i]] as List<string>, targetHeaders[i] as string);
            }

            // Write the string to a file.
            StreamWriter file = new System.IO.StreamWriter(writeFile);

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
            file.WriteLine(compNames.ToString());
            file.WriteLine(headers.ToString());
            foreach (PostcodePair item in cc.Results.Values)
            {
                file.WriteLine(item.ToString());
            }
            file.Close();
            DateTime endTime = DateTime.Now;

            Console.WriteLine(string.Format("Start time: {0}; End time: {1}; Elapsed time: {2}", startTime, endTime, endTime.Subtract(startTime)));
            Console.ReadLine();
        }
    }
}
