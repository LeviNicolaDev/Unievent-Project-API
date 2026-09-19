using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioUnievent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsuarioUnievent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Chave = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    RoleUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TentativasLogin = table.Column<int>(type: "int", nullable: false),
                    IsAtivo = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioUnievent", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioUnievent_Chave",
                table: "UsuarioUnievent",
                column: "Chave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioUnievent_EmailUsuario",
                table: "UsuarioUnievent",
                column: "EmailUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioUnievent");
        }
    }
}
