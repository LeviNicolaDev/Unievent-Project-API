using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiInstituicao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Evento_InstituicaoId",
                table: "Evento");

            migrationBuilder.AddColumn<int>(
                name: "InstituicaoId",
                table: "UsuarioSecretaria",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstituicaoId",
                table: "ResponsavelEvento",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AtualizadoEmUtc",
                table: "Instituicao",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Instituicao",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CriadoEmUtc",
                table: "Instituicao",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<bool>(
                name: "IsAtivo",
                table: "Instituicao",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Instituicao",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeAbreviado",
                table: "Instituicao",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "Instituicao",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "Instituicao",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Visibilidade",
                table: "Evento",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PublicoPermitido",
                table: "Evento",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "Evento",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSecretaria_InstituicaoId",
                table: "UsuarioSecretaria",
                column: "InstituicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ResponsavelEvento_InstituicaoId",
                table: "ResponsavelEvento",
                column: "InstituicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Instituicao_Codigo",
                table: "Instituicao",
                column: "Codigo",
                unique: true,
                filter: "[Codigo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_InstituicaoId_Categoria",
                table: "Evento",
                columns: new[] { "InstituicaoId", "Categoria" });

            migrationBuilder.CreateIndex(
                name: "IX_Evento_InstituicaoId_DataEvento",
                table: "Evento",
                columns: new[] { "InstituicaoId", "DataEvento" });

            migrationBuilder.CreateIndex(
                name: "IX_Evento_Visibilidade_PublicoPermitido",
                table: "Evento",
                columns: new[] { "Visibilidade", "PublicoPermitido" });

            migrationBuilder.AddForeignKey(
                name: "FK_ResponsavelEvento_Instituicao_InstituicaoId",
                table: "ResponsavelEvento",
                column: "InstituicaoId",
                principalTable: "Instituicao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioSecretaria_Instituicao_InstituicaoId",
                table: "UsuarioSecretaria",
                column: "InstituicaoId",
                principalTable: "Instituicao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponsavelEvento_Instituicao_InstituicaoId",
                table: "ResponsavelEvento");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioSecretaria_Instituicao_InstituicaoId",
                table: "UsuarioSecretaria");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioSecretaria_InstituicaoId",
                table: "UsuarioSecretaria");

            migrationBuilder.DropIndex(
                name: "IX_ResponsavelEvento_InstituicaoId",
                table: "ResponsavelEvento");

            migrationBuilder.DropIndex(
                name: "IX_Instituicao_Codigo",
                table: "Instituicao");

            migrationBuilder.DropIndex(
                name: "IX_Evento_InstituicaoId_Categoria",
                table: "Evento");

            migrationBuilder.DropIndex(
                name: "IX_Evento_InstituicaoId_DataEvento",
                table: "Evento");

            migrationBuilder.DropIndex(
                name: "IX_Evento_Visibilidade_PublicoPermitido",
                table: "Evento");

            migrationBuilder.DropColumn(
                name: "InstituicaoId",
                table: "UsuarioSecretaria");

            migrationBuilder.DropColumn(
                name: "InstituicaoId",
                table: "ResponsavelEvento");

            migrationBuilder.DropColumn(
                name: "AtualizadoEmUtc",
                table: "Instituicao");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Instituicao");

            migrationBuilder.DropColumn(
                name: "CriadoEmUtc",
                table: "Instituicao");

            migrationBuilder.DropColumn(
                name: "IsAtivo",
                table: "Instituicao");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Instituicao");

            migrationBuilder.DropColumn(
                name: "NomeAbreviado",
                table: "Instituicao");

            migrationBuilder.DropColumn(
                name: "Site",
                table: "Instituicao");

            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "Instituicao");

            migrationBuilder.AlterColumn<string>(
                name: "Visibilidade",
                table: "Evento",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "PublicoPermitido",
                table: "Evento",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "Evento",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_InstituicaoId",
                table: "Evento",
                column: "InstituicaoId");
        }
    }
}
