using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PlataformaAutogestion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRateHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RateHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCompany = table.Column<int>(type: "integer", nullable: false),
                    RateType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: true),
                    PreviousValue = table.Column<decimal>(type: "numeric", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdUser = table.Column<int>(type: "integer", nullable: true),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RateHistories_Companys_IdCompany",
                        column: x => x.IdCompany,
                        principalTable: "Companys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RateHistories_IdCompany_ChangedAt",
                table: "RateHistories",
                columns: new[] { "IdCompany", "ChangedAt" });

            // Valor inicial: deja registrado el valor hora vigente de cada empresa al activar el historial.
            migrationBuilder.Sql(@"INSERT INTO ""RateHistories"" (""IdCompany"", ""RateType"", ""Value"", ""PreviousValue"", ""ChangedAt"", ""IdUser"", ""UserName"")
                SELECT ""Id"", 'Normal', ""ParameterSystem"", NULL, now(), NULL, 'Valor inicial' FROM ""Companys"" WHERE ""ParameterSystem"" > 0;
                INSERT INTO ""RateHistories"" (""IdCompany"", ""RateType"", ""Value"", ""PreviousValue"", ""ChangedAt"", ""IdUser"", ""UserName"")
                SELECT ""Id"", 'Feriado', ""HolidayHourlyRate"", NULL, now(), NULL, 'Valor inicial' FROM ""Companys"" WHERE ""HolidayHourlyRate"" IS NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RateHistories");
        }
    }
}
