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
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();
            services.AddScoped<IEventoRepository, EventoRepository>();
            services.AddScoped<IInstituicaoRepository, InstituicaoRepository>();
            services.AddScoped<IResponsavelEventoRepository, ResponsavelEventoRepository>();
            services.AddScoped<IUsuarioSecretariaRepository, UsuarioSecretariaRepository>();


            return services;
        }
    }
}
