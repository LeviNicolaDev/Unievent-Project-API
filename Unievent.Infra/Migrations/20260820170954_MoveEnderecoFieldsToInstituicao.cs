using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class MoveEnderecoFieldsToInstituicao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evento_Endereco_EnderecoId",
                table: "Evento");

            migrationBuilder.DropForeignKey(
                name: "FK_Instituicao_Endereco_EnderecoId",
                table: "Instituicao");

            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "Instituicao",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cep",
                table: "Instituicao",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Instituicao",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Instituicao",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "Instituicao",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rua",
                table: "Instituicao",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE i
                SET
                    i.Bairro = LEFT(COALESCE(e.Bairro, ''), 100),
                    i.Cep = LEFT(COALESCE(e.Cep, ''), 8),
                    i.Cidade = LEFT(COALESCE(e.Cidade, ''), 100),
                    i.Estado = LEFT(COALESCE(e.Estado, ''), 50),
                    i.Numero = LEFT(COALESCE(e.Numero, ''), 20),
                    i.Rua = LEFT(COALESCE(e.Rua, ''), 200)
                FROM Instituicao i
                LEFT JOIN Endereco e ON e.Id = i.EnderecoId;
                """);

            migrationBuilder.DropIndex(
                name: "IX_Instituicao_EnderecoId",
                table: "Instituicao");

            migrationBuilder.DropIndex(
                name: "IX_Evento_EnderecoId",
                table: "Evento");

            migrationBuilder.DropColumn(
                name: "EnderecoId",
                table: "Instituicao");

            migrationBuilder.DropColumn(
                name: "EnderecoId",
                table: "Evento");

            migrationBuilder.DropTable(
                name: "Endereco");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "Instituicao");

            migrationBuilder.DropColumn(
                name: "Cep",
                table: "Instituicao");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Instituicao");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Instituicao");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Instituicao");

            migrationBuilder.DropColumn(
                name: "Rua",
                table: "Instituicao");

            migrationBuilder.AddColumn<int>(
                name: "EnderecoId",
                table: "Instituicao",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EnderecoId",
                table: "Evento",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Endereco",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Rua = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endereco", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Instituicao_EnderecoId",
                table: "Instituicao",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_EnderecoId",
                table: "Evento",
                column: "EnderecoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Evento_Endereco_EnderecoId",
                table: "Evento",
                column: "EnderecoId",
                principalTable: "Endereco",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Instituicao_Endereco_EnderecoId",
                table: "Instituicao",
                column: "EnderecoId",
                principalTable: "Endereco",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
