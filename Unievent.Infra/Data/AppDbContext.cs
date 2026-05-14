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

        modelBuilder.Entity<UsuarioSecretaria>()
            .HasIndex(s => s.Chave)
            .IsUnique();
        modelBuilder.Entity<UsuarioSecretaria>()
            .HasIndex(s => s.EmailUsuario)
            .IsUnique();
        modelBuilder.Entity<Aluno>()
            .HasIndex(a => a.Email)
            .IsUnique();
        modelBuilder.Entity<UsuarioSecretaria>()
            .Property(s => s.Chave)
            .HasMaxLength(300);
        modelBuilder.Entity<Instituicao>()
            .Property(i => i.Cnpj)
            .HasMaxLength(18);
        modelBuilder.Entity<Endereco>()
            .Property(e => e.Cep)
            .HasMaxLength(8);
        modelBuilder.Entity<Endereco>()
            .Property(e => e.Estado)
            .HasMaxLength(2);
        modelBuilder.Entity<Endereco>()
            .Property(e => e.Numero)
            .HasMaxLength(5);
        modelBuilder.Entity<UsuarioSecretaria>()
            .HasQueryFilter(s => s.IsAtivo);
        modelBuilder.Entity<Aluno>()
       .HasQueryFilter(s => s.IsAtivo);
        modelBuilder.Entity<Evento>()
        .Property(e => e.Categoria).HasConversion<string>();
    }
}
