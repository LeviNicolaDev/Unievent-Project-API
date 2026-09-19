using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aluno",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FotoPerfil = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAtivo = table.Column<bool>(type: "bit", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aluno", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Endereco",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rua = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endereco", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResponsavelEvento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FotoPerfil = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsavelEvento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioSecretaria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleUsuario = table.Column<int>(type: "int", nullable: false),
                    EmailUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Chave = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TentativasLogin = table.Column<int>(type: "int", nullable: false),
                    IsAtivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioSecretaria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instituicao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailLogin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenhaLogin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FotoPerfil = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    IdEndereco = table.Column<int>(type: "int", nullable: false),
                    EnderecoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instituicao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instituicao_Endereco_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "Endereco",
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
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoraEvento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdResponsavelEvento = table.Column<int>(type: "int", nullable: false),
                    ResponsavelEventoId = table.Column<int>(type: "int", nullable: false),
                    Capacidade = table.Column<int>(type: "int", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evento", x => x.Id);
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
                    AlunoId = table.Column<int>(type: "int", nullable: false),
                    IdAluno = table.Column<int>(type: "int", nullable: false),
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    IdEvento = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificado_Aluno_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Aluno",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Certificado_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aluno_Email",
                table: "Aluno",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificado_AlunoId",
                table: "Certificado",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificado_EventoId",
                table: "Certificado",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_ResponsavelEventoId",
                table: "Evento",
                column: "ResponsavelEventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Instituicao_EnderecoId",
                table: "Instituicao",
                column: "EnderecoId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificado");

            migrationBuilder.DropTable(
                name: "Instituicao");

            migrationBuilder.DropTable(
                name: "UsuarioSecretaria");

            migrationBuilder.DropTable(
                name: "Aluno");

            migrationBuilder.DropTable(
                name: "Evento");

            migrationBuilder.DropTable(
                name: "Endereco");

            migrationBuilder.DropTable(
                name: "ResponsavelEvento");
        }
    }
}
