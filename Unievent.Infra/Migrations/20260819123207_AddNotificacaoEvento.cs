using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificacaoEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_NotificacaoEvento_AlunoId_EventoId_Tipo",
                table: "NotificacaoEvento",
                columns: new[] { "AlunoId", "EventoId", "Tipo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificacaoEvento_EventoId",
                table: "NotificacaoEvento",
                column: "EventoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificacaoEvento");
        }
    }
}
