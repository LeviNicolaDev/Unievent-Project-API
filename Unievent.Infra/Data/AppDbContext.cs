using Microsoft.EntityFrameworkCore;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;

namespace Unievent.Infra.Data;

public class AppDbContext : DbContext
{


    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<Aluno> Aluno { get; set; }
    public DbSet<UsuarioUnievent> UsuarioUnievent { get; set; }
    public DbSet<UsuarioSecretaria> UsuarioSecretaria { get; set; }
    public DbSet<Evento> Evento { get; set; }
    public DbSet<Certificado> Certificado { get; set; }
    public DbSet<ResponsavelEvento> ResponsavelEvento { get; set; }
    public DbSet<Participacao> Participacao { get; set; }
    public DbSet<Instituicao> Instituicao { get; set; }
    public DbSet<PreferenciaNotificacao> PreferenciaNotificacao { get; set; }
    public DbSet<NotificacaoEvento> NotificacaoEvento { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 'HasDefaultValue' aparentemente não funciona mais com o EF CORE, então a inicialização de 'TentativasLogin' deve ser feita no construtor da entidade 'Usuario
        //modelBuilder.Entity<UsuarioSecretaria>()
        //    .Property(s => s.TentativasLogin)
        //    .HasDefaultValue(0);

        modelBuilder.Entity<UsuarioUnievent>(u =>
        {
            u.HasIndex(s => s.EmailUsuario).IsUnique();
            u.HasIndex(s => s.Chave).IsUnique();
            u.Property(s => s.Chave).HasMaxLength(300);
            u.Property(s => s.RoleUsuario).HasConversion<string>();
            u.HasQueryFilter(s => s.IsAtivo);
        });

        modelBuilder.Entity<UsuarioSecretaria>(u =>
        {
            u.HasIndex(s => s.EmailUsuario).IsUnique();
            u.HasIndex(s => s.Chave).IsUnique();
            u.HasIndex(s => s.InstituicaoId);
            u.Property(s => s.Chave).HasMaxLength(300);
            u.Property(s => s.RoleUsuario).HasConversion<string>();
            u.Property(s => s.Status).HasConversion<string>();
            u.HasOne(s => s.Instituicao).WithMany().HasForeignKey(s => s.InstituicaoId).OnDelete(DeleteBehavior.Restrict);
            u.HasQueryFilter(s => s.IsAtivo);

        });
        modelBuilder.Entity<Aluno>(a =>
        {
            a.HasIndex(a => a.Email).IsUnique();
            a.HasIndex(a => a.ChaveConfirmacaoEmail).IsUnique().HasFilter("[ChaveConfirmacaoEmail] IS NOT NULL");
            a.Property(a => a.ChaveConfirmacaoEmail).HasMaxLength(300);
            a.HasQueryFilter(a => a.IsAtivo);
            a.Property(a => a.TipoParticipante).HasConversion<string>();
            a.HasOne(a => a.Instituicao).WithMany().HasForeignKey(a => a.InstituicaoId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Instituicao>(i =>
        {
            i.HasIndex(i => i.Cnpj).IsUnique();
            i.HasIndex(i => i.Codigo).IsUnique().HasFilter("[Codigo] IS NOT NULL");
            i.Property(i => i.Cnpj).HasMaxLength(18);
            i.Property(i => i.Nome).HasMaxLength(160);
            i.Property(i => i.NomeAbreviado).HasMaxLength(40);
            i.Property(i => i.Codigo).HasMaxLength(60);
            i.Property(i => i.Rua).HasMaxLength(200);
            i.Property(i => i.Cidade).HasMaxLength(100);
            i.Property(i => i.Bairro).HasMaxLength(100);
            i.Property(i => i.Estado).HasMaxLength(50);
            i.Property(i => i.Cep).HasMaxLength(8);
            i.Property(i => i.Numero).HasMaxLength(20);
            i.Property(i => i.Telefone).HasMaxLength(30);
            i.Property(i => i.Site).HasMaxLength(200);
            i.HasQueryFilter(i => i.IsAtivo);
        });
        modelBuilder.Entity<ResponsavelEvento>(r =>
        {
            r.HasIndex(x => x.InstituicaoId);
            r.HasOne(x => x.Instituicao).WithMany().HasForeignKey(x => x.InstituicaoId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Evento>(e =>
        {
            e.Property(x => x.Categoria).HasConversion<string>();
            e.Property(x => x.Local).HasMaxLength(200);
            e.Property(x => x.Visibilidade).HasConversion<string>();
            e.Property(x => x.PublicoPermitido).HasConversion<string>();
            e.HasOne(x => x.Instituicao).WithMany().HasForeignKey(x => x.InstituicaoId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.InstituicaoId, x.DataEvento });
            e.HasIndex(x => new { x.InstituicaoId, x.Categoria });
            e.HasIndex(x => new { x.Visibilidade, x.PublicoPermitido });
        });
        modelBuilder.Entity<Participacao>(p =>
        {
            p.HasIndex(x => new { x.AlunoId, x.EventoId }).IsUnique();
            p.HasIndex(x => x.CodigoIngresso).IsUnique();
            p.Property(x => x.StatusInscricao)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(StatusInscricao.Ativa);
            p.Property(x => x.ErroEnvioCertificadoEmail).HasMaxLength(500);
            p.Property(x => x.NomeArquivoCertificado).HasMaxLength(180);
            p.Property(x => x.DestinatarioCertificadoEmail).HasMaxLength(320);
            p.Property(x => x.StatusEnvioCertificado).HasConversion<string>().HasMaxLength(30);
            p.HasIndex(x => new { x.StatusEnvioCertificado, x.ProximaTentativaCertificadoUtc });
        });
        modelBuilder.Entity<PreferenciaNotificacao>(p =>
        {
            p.HasIndex(x => x.AlunoId).IsUnique();
            p.HasOne(x => x.Aluno).WithOne().HasForeignKey<PreferenciaNotificacao>(x => x.AlunoId);
        });
        modelBuilder.Entity<NotificacaoEvento>(n =>
        {
            n.HasIndex(x => new { x.AlunoId, x.EventoId, x.Tipo }).IsUnique();
            n.Property(x => x.Tipo).HasMaxLength(60);
            n.Property(x => x.Assunto).HasMaxLength(200);
            n.HasOne(x => x.Aluno).WithMany().HasForeignKey(x => x.AlunoId).OnDelete(DeleteBehavior.Cascade);
            n.HasOne(x => x.Evento).WithMany().HasForeignKey(x => x.EventoId).OnDelete(DeleteBehavior.Cascade);
        });

    }
}
