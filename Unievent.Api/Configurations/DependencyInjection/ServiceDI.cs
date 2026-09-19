using Unievent.Application.Interfaces.Services;
using Unievent.Application.Services;
using Unievent.Infra.Services;

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
            services.AddScoped<ICertificadoService, CertificadoService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IParticipacaoService, ParticipacaoService>();
            services.AddScoped<IAutomacaoEventosService, AutomacaoEventosService>();
            services.AddScoped<ICertificadoAutomaticoService, CertificadoAutomaticoService>();
            services.AddSingleton<ICertificadoPdfGenerator, CertificadoPdfGenerator>();

            return services;
        }
    }
}
