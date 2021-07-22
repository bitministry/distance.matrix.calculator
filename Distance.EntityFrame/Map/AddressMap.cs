using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Distance.Business.Entitiy;

namespace Distance.EntityFrame.Map
{
    public class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            ToTable("Address");
            // Primary Key
            HasKey(x => x.AddressId)
                .Property(e => e.AddressId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.PostCode)
                .IsRequired()
                .HasMaxLength(18);

            Property(x => x.CountryCodeIso3)
//                .IsRequired()
                .HasMaxLength(3);

            Property(x => x.Email)
                .HasMaxLength(66);
            Property(x => x.Phone)
                .HasMaxLength(33); 

            // Properties
            HasRequired(x => x.Contact)
                .WithMany(y => y.Addresses)
                .Map(z => z.MapKey("ContactId"));


        }
    }
}
