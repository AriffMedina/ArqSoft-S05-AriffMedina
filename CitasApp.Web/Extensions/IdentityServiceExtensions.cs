using CitasApp.Web.Data;
using Microsoft.AspNetCore.Identity;

namespace CitasApp.Web.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityConfig(this IServiceCollection services)
        {
            services.AddDefaultIdentity<IdentityUser>(options =>
                    options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }
    }
}