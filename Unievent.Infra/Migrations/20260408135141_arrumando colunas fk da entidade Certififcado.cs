using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class arrumandocolunasfkdaentidadeCertififcado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdAluno",
                table: "Certificado");

            migrationBuilder.DropColumn(
                name: "IdEvento",
                table: "Certificado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdAluno",
                table: "Certificado",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdEvento",
                table: "Certificado",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
