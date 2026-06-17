using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlataformaAutogestion.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Details_Companys_CompanyId",
                table: "Details");

            migrationBuilder.DropForeignKey(
                name: "FK_Details_Liquidations_LiquidationId",
                table: "Details");

            migrationBuilder.DropForeignKey(
                name: "FK_Details_Users_UserId",
                table: "Details");

            migrationBuilder.DropForeignKey(
                name: "FK_Liquidations_Companys_CompanyId",
                table: "Liquidations");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Companys_CompanyId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Workdays_Companys_CompanyId",
                table: "Workdays");

            migrationBuilder.DropForeignKey(
                name: "FK_Workdays_Users_UsuarioId",
                table: "Workdays");

            migrationBuilder.DropIndex(
                name: "IX_Workdays_CompanyId",
                table: "Workdays");

            migrationBuilder.DropIndex(
                name: "IX_Workdays_UsuarioId",
                table: "Workdays");

            migrationBuilder.DropIndex(
                name: "IX_Users_CompanyId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Liquidations_CompanyId",
                table: "Liquidations");

            migrationBuilder.DropIndex(
                name: "IX_Details_CompanyId",
                table: "Details");

            migrationBuilder.DropIndex(
                name: "IX_Details_LiquidationId",
                table: "Details");

            migrationBuilder.DropIndex(
                name: "IX_Details_UserId",
                table: "Details");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Workdays");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Workdays");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Liquidations");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Details");

            migrationBuilder.DropColumn(
                name: "LiquidationId",
                table: "Details");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Details");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateApproval",
                table: "Workdays",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Workdays",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companys",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
            
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreationDate", "Email", "IdCompany", "Name", "Password", "UserName", "role" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@empresa.com", 1, "Administrador Sistema", "hashed_password_placeholder", "admin", 0 });
            

            migrationBuilder.CreateIndex(
                name: "IX_Workdays_IdCompany",
                table: "Workdays",
                column: "IdCompany");

            migrationBuilder.CreateIndex(
                name: "IX_Workdays_IdUser",
                table: "Workdays",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdCompany",
                table: "Users",
                column: "IdCompany");

            migrationBuilder.CreateIndex(
                name: "IX_Liquidations_IdCompany",
                table: "Liquidations",
                column: "IdCompany");

            migrationBuilder.CreateIndex(
                name: "IX_Details_IdCompany",
                table: "Details",
                column: "IdCompany");

            migrationBuilder.CreateIndex(
                name: "IX_Details_IdLiquidation",
                table: "Details",
                column: "IdLiquidation");

            migrationBuilder.CreateIndex(
                name: "IX_Details_IdUser",
                table: "Details",
                column: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Details_Companys_IdCompany",
                table: "Details",
                column: "IdCompany",
                principalTable: "Companys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Details_Liquidations_IdLiquidation",
                table: "Details",
                column: "IdLiquidation",
                principalTable: "Liquidations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Details_Users_IdUser",
                table: "Details",
                column: "IdUser",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Liquidations_Companys_IdCompany",
                table: "Liquidations",
                column: "IdCompany",
                principalTable: "Companys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Companys_IdCompany",
                table: "Users",
                column: "IdCompany",
                principalTable: "Companys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Workdays_Companys_IdCompany",
                table: "Workdays",
                column: "IdCompany",
                principalTable: "Companys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Workdays_Users_IdUser",
                table: "Workdays",
                column: "IdUser",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Details_Companys_IdCompany",
                table: "Details");

            migrationBuilder.DropForeignKey(
                name: "FK_Details_Liquidations_IdLiquidation",
                table: "Details");

            migrationBuilder.DropForeignKey(
                name: "FK_Details_Users_IdUser",
                table: "Details");

            migrationBuilder.DropForeignKey(
                name: "FK_Liquidations_Companys_IdCompany",
                table: "Liquidations");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Companys_IdCompany",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Workdays_Companys_IdCompany",
                table: "Workdays");

            migrationBuilder.DropForeignKey(
                name: "FK_Workdays_Users_IdUser",
                table: "Workdays");

            migrationBuilder.DropIndex(
                name: "IX_Workdays_IdCompany",
                table: "Workdays");

            migrationBuilder.DropIndex(
                name: "IX_Workdays_IdUser",
                table: "Workdays");

            migrationBuilder.DropIndex(
                name: "IX_Users_IdCompany",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Liquidations_IdCompany",
                table: "Liquidations");

            migrationBuilder.DropIndex(
                name: "IX_Details_IdCompany",
                table: "Details");

            migrationBuilder.DropIndex(
                name: "IX_Details_IdLiquidation",
                table: "Details");

            migrationBuilder.DropIndex(
                name: "IX_Details_IdUser",
                table: "Details");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companys",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateApproval",
                table: "Workdays",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Workdays",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Workdays",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Workdays",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Liquidations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Details",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LiquidationId",
                table: "Details",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Details",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companys",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.CreateIndex(
                name: "IX_Workdays_CompanyId",
                table: "Workdays",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Workdays_UsuarioId",
                table: "Workdays",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyId",
                table: "Users",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Liquidations_CompanyId",
                table: "Liquidations",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Details_CompanyId",
                table: "Details",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Details_LiquidationId",
                table: "Details",
                column: "LiquidationId");

            migrationBuilder.CreateIndex(
                name: "IX_Details_UserId",
                table: "Details",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Details_Companys_CompanyId",
                table: "Details",
                column: "CompanyId",
                principalTable: "Companys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Details_Liquidations_LiquidationId",
                table: "Details",
                column: "LiquidationId",
                principalTable: "Liquidations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Details_Users_UserId",
                table: "Details",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Liquidations_Companys_CompanyId",
                table: "Liquidations",
                column: "CompanyId",
                principalTable: "Companys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Companys_CompanyId",
                table: "Users",
                column: "CompanyId",
                principalTable: "Companys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workdays_Companys_CompanyId",
                table: "Workdays",
                column: "CompanyId",
                principalTable: "Companys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workdays_Users_UsuarioId",
                table: "Workdays",
                column: "UsuarioId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
