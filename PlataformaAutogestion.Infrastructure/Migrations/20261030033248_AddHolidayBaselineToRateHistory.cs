using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlataformaAutogestion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHolidayBaselineToRateHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Registros de feriado hechos con la versión anterior (valor vacío = "el doble"): se pasan a montos,
            // usando el valor hora normal que regía en ese momento.
            migrationBuilder.Sql(@"UPDATE ""RateHistories"" r
                SET ""Value"" = COALESCE(r.""Value"", n.normal * 2),
                    ""PreviousValue"" = COALESCE(r.""PreviousValue"", n.normal * 2)
                FROM (
                    SELECT h.""Id"" AS id,
                           (SELECT x.""Value"" FROM ""RateHistories"" x
                            WHERE x.""IdCompany"" = h.""IdCompany"" AND x.""RateType"" = 'Normal' AND x.""ChangedAt"" <= h.""ChangedAt""
                            ORDER BY x.""ChangedAt"" DESC, x.""Id"" DESC LIMIT 1) AS normal
                    FROM ""RateHistories"" h
                    WHERE h.""RateType"" = 'Feriado'
                ) n
                WHERE r.""Id"" = n.id
                  AND n.normal IS NOT NULL
                  AND r.""RateType"" = 'Feriado'
                  AND (r.""Value"" IS NULL OR (r.""PreviousValue"" IS NULL AND r.""UserName"" IS DISTINCT FROM 'Valor inicial'));");

            // Valor inicial del feriado: el monto que cada empresa paga hoy (el propio o el doble del valor hora),
            // para que el historial muestre siempre montos y no la forma de calcularlos.
            migrationBuilder.Sql(@"INSERT INTO ""RateHistories"" (""IdCompany"", ""RateType"", ""Value"", ""PreviousValue"", ""ChangedAt"", ""IdUser"", ""UserName"")
                SELECT c.""Id"", 'Feriado', COALESCE(c.""HolidayHourlyRate"", c.""ParameterSystem"" * 2), NULL, now(), NULL, 'Valor inicial'
                FROM ""Companys"" c
                WHERE c.""ParameterSystem"" > 0
                  AND NOT EXISTS (SELECT 1 FROM ""RateHistories"" r WHERE r.""IdCompany"" = c.""Id"" AND r.""RateType"" = 'Feriado');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
