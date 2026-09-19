using Unievent.Application.Interfaces.Repository;
using Unievent.Infra.Repository;

namespace Unievent.Api.Configurations.DependencyInjection
{
    public static class RepositoryDI
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAlunoRepository, AlunoRepository>();
            services.AddScoped<ICertificadoRepository, CertificadoRepository>();
            services.AddScoped<IEventoRepository, EventoRepository>();
            services.AddScoped<IInstituicaoRepository, InstituicaoRepository>();
            services.AddScoped<IResponsavelEventoRepository, ResponsavelEventoRepository>();
            services.AddScoped<IUsuarioUnieventRepository, UsuarioUnieventRepository>();
            services.AddScoped<IUsuarioSecretariaRepository, UsuarioSecretariaRepository>();
            services.AddScoped<IParticipacaoRepository, ParticipacaoRepository>();


            return services;
        }
    }
}
