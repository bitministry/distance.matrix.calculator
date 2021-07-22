using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distance.Business.Entitiy
{
    [Serializable]
    public class Span
    {
        private string _combinedId;
        public string CombinedId {
            get
            {
                if (_combinedId == null)
                {
                    var combine = new[]
                    {
                        Origin.PostCode, 
                        Destination.PostCode
                    };
                    Array.Sort( combine );
                    _combinedId = (combine[0] +"*"+ combine[1]).Replace(" ", "");
                }
                return _combinedId; 
            }
            set { _combinedId = value; }
        }
        public Address Origin { get; set; }
        public Address Destination { get; set; }
        public int? Meter { get; set; }
        public int Second { get; set; }

        public string Error { get; set; }

        public DateTime Created { get; set; }
        public bool Valid {
            get { return Created.AddMinutes( Statics.SpanValidityInMinutes) > DateTime.Now ; }
        }

    }
}
