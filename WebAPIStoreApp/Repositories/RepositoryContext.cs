using Microsoft.EntityFrameworkCore;
using WebAPIStoreApp.Models;
using WebAPIStoreApp.Repositories.Config;

namespace WebAPIStoreApp.Repositories
{
    public class RepositoryContext:DbContext
    {
        public RepositoryContext(DbContextOptions options):base(options)
        {

        }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // model oluşturulurken BookConfig'de yazdığımız çekirdek veriler doğrudan veritabanına yansıyacak
            modelBuilder.ApplyConfiguration(new BookConfig());
        }
    }
}
