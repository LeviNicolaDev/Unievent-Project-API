using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddAutomacoesERegrasEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participacao_AlunoId",
                table: "Participacao");

            migrationBuilder.AddColumn<string>(
                name: "CodigoIngresso",
                table: "Participacao",
                type: "nvarchar(450)",
                nullable: false,
                defaultValueSql: "LOWER(REPLACE(CONVERT(varchar(36), NEWID()), '-', ''))");

            migrationBuilder.AddColumn<double>(
                name: "DistanciaCheckInMetros",
                table: "Participacao",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OperadorCheckInId",
                table: "Participacao",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PrecisaoLocalizacaoMetros",
                table: "Participacao",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnderecoId",
                table: "Evento",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FimInscricoes",
                table: "Evento",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InicioInscricoes",
                table: "Evento",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituicaoId",
                table: "Evento",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Evento",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Evento",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicoPermitido",
                table: "Evento",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Todos");

            migrationBuilder.AddColumn<int>(
                name: "RaioCheckInMetros",
                table: "Evento",
                type: "int",
                nullable: false,
                defaultValue: 150);

            migrationBuilder.AddColumn<int>(
                name: "ToleranciaCheckInMinutos",
                table: "Evento",
                type: "int",
                nullable: false,
                defaultValue: 60);

            migrationBuilder.AddColumn<string>(
                name: "Visibilidade",
                table: "Evento",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Publico");

            migrationBuilder.AddColumn<int>(
                name: "InstituicaoId",
                table: "Aluno",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoParticipante",
                table: "Aluno",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Interno");

            migrationBuilder.CreateTable(
                name: "PreferenciaNotificacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlunoId = table.Column<int>(type: "int", nullable: false),
                    LembretesEventos = table.Column<bool>(type: "bit", nullable: false),
                    AlertasCertificados = table.Column<bool>(type: "bit", nullable: false),
                    Recomendacoes = table.Column<bool>(type: "bit", nullable: false),
                    UsarLocalizacao = table.Column<bool>(type: "bit", nullable: false),
                    Categorias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatitudeAproximada = table.Column<double>(type: "float", nullable: true),
                    LongitudeAproximada = table.Column<double>(type: "float", nullable: true),
                    RaioKm = table.Column<int>(type: "int", nullable: false),
                    AtualizadoEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferenciaNotificacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreferenciaNotificacao_Aluno_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Aluno",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Participacao_AlunoId_EventoId",
                table: "Participacao",
                columns: new[] { "AlunoId", "EventoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participacao_CodigoIngresso",
                table: "Participacao",
                column: "CodigoIngresso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evento_EnderecoId",
                table: "Evento",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_InstituicaoId",
                table: "Evento",
                column: "InstituicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Aluno_InstituicaoId",
                table: "Aluno",
                column: "InstituicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PreferenciaNotificacao_AlunoId",
                table: "PreferenciaNotificacao",
                column: "AlunoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Aluno_Instituicao_InstituicaoId",
                table: "Aluno",
                column: "InstituicaoId",
                principalTable: "Instituicao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evento_Endereco_EnderecoId",
                table: "Evento",
                column: "EnderecoId",
                principalTable: "Endereco",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evento_Instituicao_InstituicaoId",
                table: "Evento",
                column: "InstituicaoId",
                principalTable: "Instituicao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aluno_Instituicao_InstituicaoId",
                table: "Aluno");

            migrationBuilder.DropForeignKey(
                name: "FK_Evento_Endereco_EnderecoId",
                table: "Evento");

            migrationBuilder.DropForeignKey(
                name: "FK_Evento_Instituicao_InstituicaoId",
                table: "Evento");

            migrationBuilder.DropTable(
                name: "PreferenciaNotificacao");

            migrationBuilder.DropIndex(
                name: "IX_Participacao_AlunoId_EventoId",
                table: "Participacao");

            migrationBuilder.DropIndex(
                name: "IX_Participacao_CodigoIngresso",
                table: "Participacao");

            migrationBuilder.DropIndex(
                name: "IX_Evento_EnderecoId",
                table: "Evento");

            migrationBuilder.DropIndex(
                name: "IX_Evento_InstituicaoId",
                table: "Evento");

            migrationBuilder.DropIndex(
                name: "IX_Aluno_InstituicaoId",
                table: "Aluno");

            migrationBuilder.DropColumn(
                name: "CodigoIngresso",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "DistanciaCheckInMetros",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "OperadorCheckInId",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "PrecisaoLocalizacaoMetros",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "EnderecoId",
                table: "Evento");

            migrationBuilder.DropColumn(
                name: "FimInscricoes",
                table: "Evento");

            migrationBuilder.DropColumn(
                name: "InicioInscricoes",
                table: "Evento");

            migrationBuilder.DropColumn(
                name: "InstituicaoId",
                table: "Evento");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Evento");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Evento");

            migrationBuilder.DropColumn(
                name: "PublicoPermitido",
                table: "Evento");

            migrationBuilder.DropColumn(
                name: "RaioCheckInMetros",
                table: "Evento");

            migrationBuilder.DropColumn(
                name: "ToleranciaCheckInMinutos",
                table: "Evento");

            migrationBuilder.DropColumn(
                name: "Visibilidade",
                table: "Evento");

            migrationBuilder.DropColumn(
                name: "InstituicaoId",
                table: "Aluno");

            migrationBuilder.DropColumn(
                name: "TipoParticipante",
                table: "Aluno");

            migrationBuilder.CreateIndex(
                name: "IX_Participacao_AlunoId",
                table: "Participacao",
                column: "AlunoId");
        }
    }
}
