using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distance.Business.Entitiy
{
    public partial class DistanceComparison
    {
        private int _elements;
        public int Elements()
        {
            if (_elements > 0) return _elements;
            _elements = Statics.EnterpriseRootContact.Addresses.Count * Customer.Addresses.Count; 
            foreach (var contact in CompetitorsIncluded )
            {
                _elements += contact.Addresses.Count*Customer.Addresses.Count; 
            }
            return _elements;
        }
    }
}
