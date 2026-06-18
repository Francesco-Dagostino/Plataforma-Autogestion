using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlataformaAutogestion.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserIdCompanyNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companys",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "IdCompany",
                table: "Users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdCompany",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Companys",
                columns: new[] { "Id", "Cuit", "DateHigh", "Name", "ParameterSystem" },
                values: new object[] { 1, 203040506079L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mi Primera Empresa PYME", 1 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreationDate", "Email", "IdCompany", "Name", "Password", "UserName", "role" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@empresa.com", 1, "Administrador Sistema", "hashed_password_placeholder", "admin", 0 });
        }
    }
}
