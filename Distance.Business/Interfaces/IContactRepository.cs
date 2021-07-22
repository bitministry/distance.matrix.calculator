using Distance.Business.Entitiy;

namespace Distance.Business.Interfaces
{
    public interface IContactRepository : IGenericRepository<Contact>
    {
        void AddAddressFor(Address address, Contact contact);
        void RemoveAddressFrom(Address address, Contact contact);
    }
}
