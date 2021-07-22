using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Distance.Business.Entitiy;

namespace Distance.EntityFrame.Map
{
    public class ComparisonReportMap : EntityTypeConfiguration<ComparisonReport>
    {
        public ComparisonReportMap()
        {
            ToTable("ComparisonReport");
            // Primary Key
            HasKey(t => t.ComparisonReportId)
                .Property(e => e.ComparisonReportId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            Property(t => t.Serialized_ICollectionFetcherLogForTarget);
            Property(t => t.Serialized_Contact_Customer);
            Property(t => t.Created).IsRequired();


        }
    }
}
