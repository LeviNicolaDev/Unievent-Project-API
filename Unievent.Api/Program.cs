using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Unievent.Api.BackgroundServices;
using Unievent.Api.Configurations.DependencyInjection;
using Unievent.Application.Configurations.Email;
using Unievent.Application.Interfaces.Auth;
using Unievent.Application.Services;
using Unievent.Application.Validators.Aluno;
using Unievent.Domain.Enuns;
using Unievent.Infra.Auth;
using Unievent.Infra.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddRepositories();
builder.Services.AddServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173" // Vite
                , "http://localhost:8081"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<AlunoRequestValidator>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole(Role.Admin.ToString()));
    options.AddPolicy("Secretaria", policy => policy.RequireRole(Role.Secretaria.ToString()));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<Unievent.Application.Configurations.CertificacaoSettings>(
    builder.Configuration.GetSection("Certificacao"));

builder.Services.AddAuthorization();


builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

if (builder.Configuration.GetValue("Automacoes:Ativas", false))
{
    builder.Services.AddHostedService<EventosAutomationWorker>();
}

// ✅ Com security scheme
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((doc, context, _) =>
    {
        doc.Components ??= new OpenApiComponents();
        doc.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Insira o token JWT"
            }
        };
        return Task.CompletedTask;
    });
});



var app = builder.Build();

if (builder.Configuration.GetValue("Database:MigrateOnStartup", false))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
 {
     options.WithTitle("Unievent API")
            .WithTheme(ScalarTheme.Moon)
            .WithPreferredScheme("Bearer")
            .WithHttpBearerAuthentication(bearer =>
            {
                bearer.Token = "";
            });
 });
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();

public partial class Program { }
