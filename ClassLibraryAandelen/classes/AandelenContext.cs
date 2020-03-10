using System.Data.Entity;

namespace ClassLibraryAandelen.classes
{
    public class AandelenContext : DbContext
    {
        public AandelenContext()
        {
            Database.SetInitializer(new DropCreateDatabaseAlwaysAandelenContext());
        }

        public DbSet<Eigenaar> Eigenaars { get; set; }
        public DbSet<Portefeuille> Portefeuilles{ get; set; }
        public DbSet<Aandeel> Aandelen{ get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}
