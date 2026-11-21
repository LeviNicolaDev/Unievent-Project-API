using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class ConfirmacaoEmailAdminPublico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmado",
                table: "UsuarioUnievent",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Contas antigas também devem comprovar o endereço antes do próximo login.
            // O reenvio gera uma chave nova exclusivamente no servidor.
            migrationBuilder.Sql("UPDATE \"UsuarioUnievent\" SET \"Chave\" = replace(gen_random_uuid()::text, '-', '')");
            migrationBuilder.Sql("UPDATE \"Aluno\" SET \"EmailConfirmado\" = FALSE, \"ChaveConfirmacaoEmail\" = NULL WHERE \"TipoParticipante\" = 'Externo'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmado",
                table: "UsuarioUnievent");
        }
    }
}
