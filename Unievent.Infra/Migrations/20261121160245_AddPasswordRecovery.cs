using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordRecovery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecuperacaoSenha",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TipoConta = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    SenhaFingerprint = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CriadoEmUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiraEmUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsadoEmUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecuperacaoSenha", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecuperacaoSenha_TipoConta_UsuarioId_CriadoEmUtc",
                table: "RecuperacaoSenha",
                columns: new[] { "TipoConta", "UsuarioId", "CriadoEmUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_RecuperacaoSenha_TokenHash",
                table: "RecuperacaoSenha",
                column: "TokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecuperacaoSenha");
        }
    }
}
