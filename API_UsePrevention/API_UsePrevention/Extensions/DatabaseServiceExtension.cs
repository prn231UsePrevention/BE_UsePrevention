using Microsoft.EntityFrameworkCore;

namespace API_UsePrevention.Extensions
{
    public static class DatabaseServiceExtension
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString"))
                       .EnableSensitiveDataLogging()
            );
            return services;
        }
    }
}
