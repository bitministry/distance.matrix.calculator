using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Distance.Business.Entitiy;

namespace Distance.EntityFrame.Map
{
    public class ContactMap : EntityTypeConfiguration<Contact>
    {
        public ContactMap()
        {
            ToTable("Contact");
            // Primary Key
            HasKey(t => t.ContactId)
                .Property(e => e.ContactId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); 

            // Properties
            HasMany(x => x.Addresses)
                .WithRequired(y => y.Contact).Map(z => z.MapKey("ContactId"));

            Property(x => x.Email)
                .HasMaxLength(66);
            Property(x => x.Phone)
                .HasMaxLength(33); 

            Property(t => t.Name)
               .IsRequired()
               .HasMaxLength(255);

            Property(t => t.AddressesModified);

            Property(t => t.ContactType)
               .IsRequired();

            
            // IGNOOOOOR !!!! 
//            Ignore(t => t.DistanceComparisons); 
        }
    }
}
