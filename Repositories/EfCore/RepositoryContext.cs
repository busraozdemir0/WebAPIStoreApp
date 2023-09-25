using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repositories.EfCore.Config;
using System.Reflection;

namespace Repositories.EfCore
{
    public class RepositoryContext : IdentityDbContext<User> // User tablosu oluşturup IdentityUser kalıttığımız için buraya geçtik
    {
        public RepositoryContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // model oluşturulurken BookConfig'de yazdığımız çekirdek veriler doğrudan veritabanına yansıyacak
            base.OnModelCreating(modelBuilder);

            //modelBuilder.ApplyConfiguration(new BookConfig());
            //modelBuilder.ApplyConfiguration(new RoleConfiguration());  // roller için çekirdek datalar yer alıyor
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());  // yukarıdaki gibi tek tek configleri yazmak yerine bu şekilde tek satırda yapabiliriz
        }
    }
}
