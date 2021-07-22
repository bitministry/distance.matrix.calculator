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
        public int PercentageCompleted()
        {
            if (ProcessedElements == 0) return 0;
            return (int)( ProcessedElements/ (double)DistanceComparison.Elements() * 100 );

        }


    }
}
