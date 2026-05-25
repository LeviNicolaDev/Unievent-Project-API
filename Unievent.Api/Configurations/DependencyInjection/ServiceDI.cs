using Unievent.Application.Interfaces.Services;
using Unievent.Application.Services;

namespace Unievent.Api.Configurations.DependencyInjection
{
    public static class ServiceDI
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IEventoService, EventoService>();
            services.AddScoped<IResponsavelEventoService, ResponsavelEventoService>();
            services.AddScoped<IUsuarioSecretariaService, UsuarioSecretariaService>();
            services.AddScoped<IAlunoService, AlunoService>();
            services.AddScoped<IInstituicaoService, InstituicaoService>();
            services.AddScoped<IEnderecoService, EnderecoService>();
            services.AddScoped<ICertificadoService, CertificadoService>();
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
