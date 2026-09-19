using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyEventoPublicoPermitido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [Evento]
                SET [PublicoPermitido] = N'PublicoGeral'
                WHERE [PublicoPermitido] IN (N'Todos', N'SomenteExternos')
                   OR [PublicoPermitido] IS NULL;

                UPDATE [Evento]
                SET [PublicoPermitido] = N'TodosAlunosFatec'
                WHERE [PublicoPermitido] = N'SomenteInternos';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [Evento]
                SET [PublicoPermitido] = N'Todos'
                WHERE [PublicoPermitido] = N'PublicoGeral';

                UPDATE [Evento]
                SET [PublicoPermitido] = N'SomenteInternos'
                WHERE [PublicoPermitido] = N'TodosAlunosFatec';
                """);
        }
    }
}
