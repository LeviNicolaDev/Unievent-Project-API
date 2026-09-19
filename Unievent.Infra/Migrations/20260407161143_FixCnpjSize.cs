using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class FixCnpjSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdEndereco",
                table: "Instituicao");

            migrationBuilder.AlterColumn<string>(
                name: "Cnpj",
                table: "Instituicao",
                type: "nvarchar(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(14)",
                oldMaxLength: 14);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Cnpj",
                table: "Instituicao",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(18)",
                oldMaxLength: 18);

            migrationBuilder.AddColumn<int>(
                name: "IdEndereco",
                table: "Instituicao",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
