using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distance.Business.Entitiy
{
    public partial class DistanceComparison
    {
        public int DistanceComparisonId { get; set; }

        public virtual Contact Customer { get; set; }

        public virtual IList<Contact> CompetitorsIncluded { get; set; }
        public virtual IList<ComparisonReport> ComparisonReports { get; set; }

    }
}
