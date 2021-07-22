using Distance.Business.Entitiy;
using Distance.EntityFrame.Map;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Distance.EntityFrame 
{
    public class MyContextFactory : IDbContextFactory<EfUnitOfWork>
    {
        public EfUnitOfWork Create()
        {
            return new EfUnitOfWork("Default");
        }
    }

    public class EfUnitOfWork : DbContext 
    {
        public EfUnitOfWork(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Initialize();
        }
        public EfUnitOfWork( string nameOrConnectionString, DbCompiledModel dbCompiledModel)
            : base( nameOrConnectionString, dbCompiledModel)
        {
            Initialize();
        }

        void Initialize()
        {
            Database.SetInitializer(new NullDatabaseInitializer<EfUnitOfWork>());

        }

        //public DbSet<Address> Countries { get; set; }
        //public DbSet<Contact> Societies { get; set; }
 
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            PopulateDbModelBuilder(modelBuilder);
        }

        public static void PopulateDbModelBuilder(DbModelBuilder modelBuilder )
        {
            modelBuilder.Configurations.Add(new DistanceComparisonMap());
            modelBuilder.Configurations.Add(new AddressMap());
            modelBuilder.Configurations.Add(new ContactMap());

        }

    }
}
