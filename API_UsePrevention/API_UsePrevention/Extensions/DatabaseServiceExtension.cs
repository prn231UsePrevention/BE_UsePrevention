using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace API_UsePrevention.Extensions
{
    public static class DatabaseServiceExtension
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DrugUsePreventionSupportSystemContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString"))
                       .EnableSensitiveDataLogging()
            );
            return services;
        }
    }
}
