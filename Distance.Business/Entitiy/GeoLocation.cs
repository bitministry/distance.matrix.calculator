using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distance.Business.Entitiy
{
    [Serializable]
    public class GeoLocation
    {
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}
