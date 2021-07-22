using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Distance.Business.Interfaces;

namespace Distance.Business.Entitiy
{
    public enum ContactType
    {
        Enterprise,
        Competitor,
        PrivateCustomer,
        BusinessCustomer 
    }

    [Serializable]
    public partial class Contact 
    {
        public Contact()
        {
            ContactType = ContactType.BusinessCustomer;
        }

        public ContactType ContactType { get; set; }

        public int ContactId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
        public int HiresPerYear { get; set; }

        private IList<Address> _addresses;
        public virtual IList<Address> Addresses  
        {
            get { return _addresses ?? (_addresses = new List<Address>()); }
            set { _addresses = value;  }
        }

        [NonSerialized] 
        private IList<DistanceComparison> _distanceComparisons;
        public IList<DistanceComparison> DistanceComparisons {
            get { return _distanceComparisons;  }
            set { _distanceComparisons = value; }
        }

        public DateTime? AddressesModified { get; set; }

        public IList<string> InvalidPostCodes;


    }
}
