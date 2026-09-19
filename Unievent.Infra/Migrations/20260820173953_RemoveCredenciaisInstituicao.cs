using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCredenciaisInstituicao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailLogin",
                table: "Instituicao");

            migrationBuilder.DropColumn(
                name: "SenhaLogin",
                table: "Instituicao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailLogin",
                table: "Instituicao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenhaLogin",
                table: "Instituicao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
