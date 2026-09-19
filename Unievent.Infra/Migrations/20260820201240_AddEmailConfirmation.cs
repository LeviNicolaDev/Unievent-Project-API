using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailConfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmado",
                table: "UsuarioSecretaria",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "ChaveConfirmacaoEmail",
                table: "Aluno",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmado",
                table: "Aluno",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aluno_ChaveConfirmacaoEmail",
                table: "Aluno",
                column: "ChaveConfirmacaoEmail",
                unique: true,
                filter: "[ChaveConfirmacaoEmail] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Aluno_ChaveConfirmacaoEmail",
                table: "Aluno");

            migrationBuilder.DropColumn(
                name: "EmailConfirmado",
                table: "UsuarioSecretaria");

            migrationBuilder.DropColumn(
                name: "ChaveConfirmacaoEmail",
                table: "Aluno");

            migrationBuilder.DropColumn(
                name: "EmailConfirmado",
                table: "Aluno");
        }
    }
}
