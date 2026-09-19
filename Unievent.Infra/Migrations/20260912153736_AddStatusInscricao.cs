using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusInscricao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataCancelamento",
                table: "Participacao",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusInscricao",
                table: "Participacao",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Ativa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataCancelamento",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "StatusInscricao",
                table: "Participacao");
        }
    }
}
