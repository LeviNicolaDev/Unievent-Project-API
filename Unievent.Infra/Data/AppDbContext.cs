using Microsoft.EntityFrameworkCore;
using Unievent.Domain.Entities;

namespace Unievent.Infra.Data;

public class AppDbContext : DbContext
{


    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<Aluno> Aluno { get; set; }
    public DbSet<UsuarioSecretaria> UsuarioSecretaria { get; set; }
    public DbSet<Evento> Evento { get; set; }
    public DbSet<Certificado> Certificado { get; set; }
    public DbSet<ResponsavelEvento> ResponsavelEvento { get; set; }
    public DbSet<Endereco> Endereco { get; set; }
    public DbSet<Instituicao> Instituicao { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 'HasDefaultValue' aparentemente não funciona mais com o EF CORE, então a inicialização de 'TentativasLogin' deve ser feita no construtor da entidade 'Usuario
        //modelBuilder.Entity<UsuarioSecretaria>()
        //    .Property(s => s.TentativasLogin)
        //    .HasDefaultValue(0);

        modelBuilder.Entity<UsuarioSecretaria>(u =>
        {
            u.HasIndex(s => s.EmailUsuario).IsUnique();
            u.HasIndex(s => s.Chave).IsUnique();
            u.Property(s => s.Chave).HasMaxLength(300);
            u.Property(s => s.RoleUsuario).HasConversion<string>();
            u.HasQueryFilter(s => s.IsAtivo);

        });
        modelBuilder.Entity<Aluno>(a =>
        {
            a.HasIndex(a => a.Email).IsUnique();
            a.HasQueryFilter(a => a.IsAtivo);
        });
        modelBuilder.Entity<Instituicao>(i =>
        {
            i.HasIndex(i => i.Cnpj).IsUnique();
            i.Property(i => i.Cnpj).HasMaxLength(18);
        });
        modelBuilder.Entity<Endereco>(e =>
        {
            e.Property(e => e.Cep).HasMaxLength(8);
            e.Property(e => e.Estado).HasMaxLength(2);
            e.Property(e => e.Numero).HasMaxLength(5);
        });
        modelBuilder.Entity<Evento>()
        .Property(e => e.Categoria).HasConversion<string>();
    }
}
