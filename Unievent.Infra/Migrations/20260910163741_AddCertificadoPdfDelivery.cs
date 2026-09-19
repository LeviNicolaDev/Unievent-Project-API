using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unievent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificadoPdfDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "CertificadoPdf",
                table: "Participacao",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataGeracaoCertificado",
                table: "Participacao",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinatarioCertificadoEmail",
                table: "Participacao",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeArquivoCertificado",
                table: "Participacao",
                type: "nvarchar(180)",
                maxLength: 180,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessamentoCertificadoAteUtc",
                table: "Participacao",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessamentoCertificadoId",
                table: "Participacao",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProximaTentativaCertificadoUtc",
                table: "Participacao",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusEnvioCertificado",
                table: "Participacao",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Pendente");

            migrationBuilder.AddColumn<int>(
                name: "TentativasEnvioCertificado",
                table: "Participacao",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Preserva envios antigos. O fluxo anterior não distinguia falha definitiva de
            // aceitação SMTP sem confirmação local; esses casos não podem ser reenviados às cegas.
            migrationBuilder.Sql("""
                UPDATE [Participacao]
                SET [StatusEnvioCertificado] = CASE
                    WHEN [CertificadoEnviadoPorEmail] = 1 THEN 'Enviado'
                    WHEN [CertificadoEmitido] = 1 THEN 'EnvioIncerto'
                    ELSE 'Pendente' END;
                UPDATE [Participacao]
                SET [ErroEnvioCertificadoEmail] = N'Emissão anterior sem confirmação de entrega; verificar no provedor antes de reenviar.'
                WHERE [StatusEnvioCertificado] = 'EnvioIncerto';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Participacao_StatusEnvioCertificado_ProximaTentativaCertificadoUtc",
                table: "Participacao",
                columns: new[] { "StatusEnvioCertificado", "ProximaTentativaCertificadoUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participacao_StatusEnvioCertificado_ProximaTentativaCertificadoUtc",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "CertificadoPdf",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "DataGeracaoCertificado",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "DestinatarioCertificadoEmail",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "NomeArquivoCertificado",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "ProcessamentoCertificadoAteUtc",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "ProcessamentoCertificadoId",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "ProximaTentativaCertificadoUtc",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "StatusEnvioCertificado",
                table: "Participacao");

            migrationBuilder.DropColumn(
                name: "TentativasEnvioCertificado",
                table: "Participacao");
        }
    }
}
