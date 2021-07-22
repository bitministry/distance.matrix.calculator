using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distance.Business.Entitiy
{
    public partial class ComparisonReport
    {
        public int ComparisonReportId { get; set; }
        public virtual DistanceComparison DistanceComparison { get; set; }

        public int ProcessedElements { get; set; }
        public string NoReplyError { get; set; }
        public byte[] Serialized_ICollectionFetcherLogForTarget { get; set; }
        public byte[] Serialized_Contact_Customer { get; set; }
        
        public DateTime Created { get; set; }

        // fields 

        public Contact CustomerWithDistances;
        public ICollection<LogForTarget> Log;

        [Serializable]
        public struct LogForTarget
        {
            public LogForTarget(Contact targetContact)
            {
                TargetContact = targetContact;
                RequestFailureLog = new HashSet<RequestFailureLogItem>();
            }

            public Contact TargetContact;
            public ICollection<RequestFailureLogItem> RequestFailureLog;

            [Serializable]
            public struct RequestFailureLogItem
            {
                public string QueryString;
                public string ExceptionMessage;
                public RequestFailureLogItem(string exceptionMessage, string queryString)
                    : this()
                {
                    ExceptionMessage = exceptionMessage;
                    QueryString = queryString;
                }
            }

        }


    }
}
