using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSO.DataAccess.Portgresql.Context;

namespace SSO.DataAccess.Portgresql {
    public static class DependencyInjection {
        public static void AddDataAccessPostgresql(this IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<ApplicationContext>(options =>
                options.UseNpgsql(configuration["Database:DefaultConnection"]));
        }
    }
}