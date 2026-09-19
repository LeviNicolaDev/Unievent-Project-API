using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class addnovatabelaparainscreverseemevento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Participacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<int>(type: "int", nullable: false),
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    PresencaConfirmada = table.Column<bool>(type: "bit", nullable: false),
                    DataConfirmacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CertificadoEmitido = table.Column<bool>(type: "bit", nullable: false),
                    CodigoValidacao = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "IX_Participacao_AlunoId",
                table: "Participacao",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_Participacao_EventoId",
                table: "Participacao",
                column: "EventoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Participacao");
        }
    }
}
