using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distance.Business.Entitiy;
using Distance.Business.Interfaces;

namespace Distance.EntityFrame.Repository
{
    public class ContactRepository : GenericRepository<Contact>, IContactRepository
    {
        private readonly IGenericRepository<Address> _addressRepository;
        public ContactRepository(EfUnitOfWork efContext, IGenericRepository<Address> addressRepository) 
            : base(efContext)
        {
            this._addressRepository = addressRepository;
        }


        public void AddAddressFor(Address address, Contact contact)
        {
            address.Contact = contact;
            contact.Addresses.Add(address);
            contact.AddressesModified = DateTime.Now;
            SaveOrUpdate( contact );
            if (_addressRepository != null)
                _addressRepository.SaveOrUpdate(address);
        }

        public void RemoveAddressFrom(Address address, Contact contact)
        {
            address.Addresses.Remove(oldAdr);
            AddressesModified = DateTime.Now;
            if (addressRepository != null)
                addressRepository.Delete(oldAdr.PostCode);
        
        }
    }
}
