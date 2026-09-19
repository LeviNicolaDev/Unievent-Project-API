using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificadoEmailTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CertificadoEnviadoPorEmail",
                table: "Participacao",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEnvioCertificadoEmail",
                table: "Participacao",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ErroEnvioCertificadoEmail",
                table: "Participacao",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificadoEnviadoPorEmail",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "DataEnvioCertificadoEmail",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "ErroEnvioCertificadoEmail",
                table: "Participacao");
        }
    }
}
