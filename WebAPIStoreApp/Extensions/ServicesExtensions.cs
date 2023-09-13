using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.EfCore;

namespace WebAPIStoreApp.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(options =>    // IoC'ye DbContext tanımını yapmış olduk (DI çerçevesi için)
                   options.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
        }

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
             services.AddScoped<IRepositoryManager, RepositoryManager>();
    }
}
