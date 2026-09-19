using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class fixtablecertificado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificado_Aluno_AlunoId",
                table: "Certificado");

            migrationBuilder.DropIndex(
                name: "IX_Certificado_AlunoId",
                table: "Certificado");

            migrationBuilder.DropColumn(
                name: "AlunoId",
                table: "Certificado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlunoId",
                table: "Certificado",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Certificado_AlunoId",
                table: "Certificado",
                column: "AlunoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificado_Aluno_AlunoId",
                table: "Certificado",
                column: "AlunoId",
                principalTable: "Aluno",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
