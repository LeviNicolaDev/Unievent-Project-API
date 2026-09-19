using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class fixconsertodacolunaRoleUsuarionatabelaUsuarioSecretaria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RoleUsuario",
                table: "UsuarioSecretaria",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Instituicao_Cnpj",
                table: "Instituicao",
                column: "Cnpj",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Instituicao_Cnpj",
                table: "Instituicao");

            migrationBuilder.AlterColumn<int>(
                name: "RoleUsuario",
                table: "UsuarioSecretaria",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
