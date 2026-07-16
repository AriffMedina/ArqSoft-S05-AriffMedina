using CitasApp.Interfaces;
using CitasApp.Repositories;

namespace CitasApp.Web.Extensions
{
    public static class RepositoryServiceExtensions
    {
        public static IServiceCollection AddDomainRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPacienteRepository, JsonPacienteRepository>();
            services.AddScoped<IMedicoRepository, JsonMedicoRepository>();
            services.AddScoped<ICitaRepository, JsonCitaRepository>();

            return services;
        }
    }
}