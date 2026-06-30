using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlataformaAutogestion.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Cuit = table.Column<long>(type: "bigint", nullable: false),
                    DateHigh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParameterSystem = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Liquidations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LiquidationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    IdCompany = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Liquidations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Liquidations_Companys_IdCompany",
                        column: x => x.IdCompany,
                        principalTable: "Companys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    role = table.Column<int>(type: "int", nullable: false),
                    IdCompany = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Companys_IdCompany",
                        column: x => x.IdCompany,
                        principalTable: "Companys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IdLiquidation = table.Column<int>(type: "int", nullable: false),
                    IdCompany = table.Column<int>(type: "int", nullable: false),
                    IdUser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Details_Companys_IdCompany",
                        column: x => x.IdCompany,
                        principalTable: "Companys",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Details_Liquidations_IdLiquidation",
                        column: x => x.IdLiquidation,
                        principalTable: "Liquidations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Details_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Workdays",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HoursWorked = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateEntry = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateApproval = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    IdCompany = table.Column<int>(type: "int", nullable: false),
                    IdUser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workdays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workdays_Companys_IdCompany",
                        column: x => x.IdCompany,
                        principalTable: "Companys",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Workdays_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Liquidations_IdCompany",
                table: "Liquidations",
                column: "IdCompany");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdCompany",
                table: "Users",
                column: "IdCompany");

            migrationBuilder.CreateIndex(
                name: "IX_Workdays_IdCompany",
                table: "Workdays",
                column: "IdCompany");

            migrationBuilder.CreateIndex(
                name: "IX_Workdays_IdUser",
                table: "Workdays",
                column: "IdUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Details");

            migrationBuilder.DropTable(
                name: "Workdays");

            migrationBuilder.DropTable(
                name: "Liquidations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Companys");
        }
    }
}
