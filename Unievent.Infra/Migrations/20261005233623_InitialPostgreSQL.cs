using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Instituicao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    NomeAbreviado = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Codigo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    FotoPerfil = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    Rua = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Site = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsAtivo = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instituicao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioUnievent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Chave = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    RoleUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TentativasLogin = table.Column<int>(type: "int", nullable: false),
                    IsAtivo = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioUnievent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Aluno",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmailConfirmado = table.Column<bool>(type: "bit", nullable: false),
                    ChaveConfirmacaoEmail = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    FotoPerfil = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAtivo = table.Column<bool>(type: "bit", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoParticipante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstituicaoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aluno", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aluno_Instituicao_InstituicaoId",
                        column: x => x.InstituicaoId,
                        principalTable: "Instituicao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResponsavelEvento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FotoPerfil = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstituicaoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsavelEvento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResponsavelEvento_Instituicao_InstituicaoId",
                        column: x => x.InstituicaoId,
                        principalTable: "Instituicao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioSecretaria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Chave = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    EmailConfirmado = table.Column<bool>(type: "bit", nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TentativasLogin = table.Column<int>(type: "int", nullable: false),
                    IsAtivo = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstituicaoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioSecretaria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioSecretaria_Instituicao_InstituicaoId",
                        column: x => x.InstituicaoId,
                        principalTable: "Instituicao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreferenciaNotificacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<int>(type: "int", nullable: false),
                    LembretesEventos = table.Column<bool>(type: "bit", nullable: false),
                    AlertasCertificados = table.Column<bool>(type: "bit", nullable: false),
                    Recomendacoes = table.Column<bool>(type: "bit", nullable: false),
                    UsarLocalizacao = table.Column<bool>(type: "bit", nullable: false),
                    Categorias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatitudeAproximada = table.Column<double>(type: "float", nullable: true),
                    LongitudeAproximada = table.Column<double>(type: "float", nullable: true),
                    RaioKm = table.Column<int>(type: "int", nullable: false),
                    AtualizadoEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferenciaNotificacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreferenciaNotificacao_Aluno_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Aluno",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Local = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Categoria = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DataEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResponsavelEventoId = table.Column<int>(type: "int", nullable: false),
                    Capacidade = table.Column<int>(type: "int", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstituicaoId = table.Column<int>(type: "int", nullable: true),
                    Visibilidade = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PublicoPermitido = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InicioInscricoes = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FimInscricoes = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    RaioCheckInMetros = table.Column<int>(type: "int", nullable: false),
                    ToleranciaCheckInMinutos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evento_Instituicao_InstituicaoId",
                        column: x => x.InstituicaoId,
                        principalTable: "Instituicao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evento_ResponsavelEvento_ResponsavelEventoId",
                        column: x => x.ResponsavelEventoId,
                        principalTable: "ResponsavelEvento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataCertifcado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificado_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificacaoEvento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<int>(type: "int", nullable: false),
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Assunto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessadaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Enviada = table.Column<bool>(type: "bit", nullable: false),
                    Erro = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacaoEvento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificacaoEvento_Aluno_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Aluno",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificacaoEvento_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<int>(type: "int", nullable: false),
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    PresencaConfirmada = table.Column<bool>(type: "bit", nullable: false),
                    StatusInscricao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Ativa"),
                    DataCancelamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataConfirmacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CertificadoEmitido = table.Column<bool>(type: "bit", nullable: false),
                    CodigoValidacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificadoEnviadoPorEmail = table.Column<bool>(type: "bit", nullable: false),
                    DataEnvioCertificadoEmail = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErroEnvioCertificadoEmail = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CertificadoPdf = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    NomeArquivoCertificado = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    DataGeracaoCertificado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DestinatarioCertificadoEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    StatusEnvioCertificado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ProcessamentoCertificadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProcessamentoCertificadoAteUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProximaTentativaCertificadoUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TentativasEnvioCertificado = table.Column<int>(type: "int", nullable: false),
                    CodigoIngresso = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DistanciaCheckInMetros = table.Column<double>(type: "float", nullable: true),
                    PrecisaoLocalizacaoMetros = table.Column<double>(type: "float", nullable: true),
                    OperadorCheckInId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participacao_Aluno_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Aluno",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participacao_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aluno_ChaveConfirmacaoEmail",
                table: "Aluno",
                column: "ChaveConfirmacaoEmail",
                unique: true,
                filter: "[ChaveConfirmacaoEmail] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Aluno_Email",
                table: "Aluno",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aluno_InstituicaoId",
                table: "Aluno",
                column: "InstituicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificado_EventoId",
                table: "Certificado",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_InstituicaoId_Categoria",
                table: "Evento",
                columns: new[] { "InstituicaoId", "Categoria" });

            migrationBuilder.CreateIndex(
                name: "IX_Evento_InstituicaoId_DataEvento",
                table: "Evento",
                columns: new[] { "InstituicaoId", "DataEvento" });

            migrationBuilder.CreateIndex(
                name: "IX_Evento_ResponsavelEventoId",
                table: "Evento",
                column: "ResponsavelEventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_Visibilidade_PublicoPermitido",
                table: "Evento",
                columns: new[] { "Visibilidade", "PublicoPermitido" });

            migrationBuilder.CreateIndex(
                name: "IX_Instituicao_Cnpj",
                table: "Instituicao",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instituicao_Codigo",
                table: "Instituicao",
                column: "Codigo",
                unique: true,
                filter: "[Codigo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacaoEvento_AlunoId_EventoId_Tipo",
                table: "NotificacaoEvento",
                columns: new[] { "AlunoId", "EventoId", "Tipo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificacaoEvento_EventoId",
                table: "NotificacaoEvento",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Participacao_AlunoId_EventoId",
                table: "Participacao",
                columns: new[] { "AlunoId", "EventoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participacao_CodigoIngresso",
                table: "Participacao",
                column: "CodigoIngresso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participacao_EventoId",
                table: "Participacao",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Participacao_StatusEnvioCertificado_ProximaTentativaCertificadoUtc",
                table: "Participacao",
                columns: new[] { "StatusEnvioCertificado", "ProximaTentativaCertificadoUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PreferenciaNotificacao_AlunoId",
                table: "PreferenciaNotificacao",
                column: "AlunoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResponsavelEvento_InstituicaoId",
                table: "ResponsavelEvento",
                column: "InstituicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSecretaria_Chave",
                table: "UsuarioSecretaria",
                column: "Chave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSecretaria_EmailUsuario",
                table: "UsuarioSecretaria",
                column: "EmailUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSecretaria_InstituicaoId",
                table: "UsuarioSecretaria",
                column: "InstituicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioUnievent_Chave",
                table: "UsuarioUnievent",
                column: "Chave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioUnievent_EmailUsuario",
                table: "UsuarioUnievent",
                column: "EmailUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificado");

            migrationBuilder.DropTable(
                name: "NotificacaoEvento");

            migrationBuilder.DropTable(
                name: "Participacao");

            migrationBuilder.DropTable(
                name: "PreferenciaNotificacao");

            migrationBuilder.DropTable(
                name: "UsuarioSecretaria");

            migrationBuilder.DropTable(
                name: "UsuarioUnievent");

            migrationBuilder.DropTable(
                name: "Evento");

            migrationBuilder.DropTable(
                name: "Aluno");

            migrationBuilder.DropTable(
                name: "ResponsavelEvento");

            migrationBuilder.DropTable(
                name: "Instituicao");
        }
    }
}
