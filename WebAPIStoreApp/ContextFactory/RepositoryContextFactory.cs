using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repositories.EfCore;

namespace WebAPIStoreApp.ContextFactory
{
    public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
    {
        public RepositoryContext CreateDbContext(string[] args)
        {
            // ConfigurationBuilder
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")  // appsettings.json dosyasına otomatik olarak erişip oradaki connectionString'i alacak
                .Build();

            // DbContextOptionsBuilder

            var builder = new DbContextOptionsBuilder<RepositoryContext>()
                .UseSqlServer(configuration.GetConnectionString("sqlConnection"), prj=> prj.MigrationsAssembly("WebAPIStoreApp"));  // appsettings.json dosyasındaki sqlConnection ile aynı olmalı ve migrations ifadelerinin artık WebAPIStoreApp içerisinde oluşmasını sağladık

            return new RepositoryContext(builder.Options);
        }
    }
}
