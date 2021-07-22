using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Distance.Business.Entitiy;

namespace Distance.EntityFrame.Map
{
    public class DistanceComparisonMap : EntityTypeConfiguration<DistanceComparison>
    {
        public DistanceComparisonMap()
        {
            ToTable("DistanceComparison");
            // Primary Key
            HasKey(t => t.DistanceComparisonId)
                .Property(e => e.DistanceComparisonId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);


            // Properties
            HasMany(c => c.CompetitorsIncluded)
                .WithMany( p => p.DistanceComparisons )
                .Map( m =>
                         {
                             m.MapLeftKey("DistanceComparisonId");
                             m.MapRightKey("ContactId");
                             m.ToTable("CompetitorsIncludedToDistanceComparison");
                         });

            HasMany(x => x.ComparisonReports)
                .WithRequired(y => y.DistanceComparison)
                .Map(z => z.MapKey("DistanceComparisonId"));

            HasRequired(x => x.Customer)
                .WithMany()
                .Map(z => z.MapKey("CustomerId"));

        }
    }
}
