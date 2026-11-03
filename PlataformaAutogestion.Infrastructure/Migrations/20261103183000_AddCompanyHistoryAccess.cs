using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using PlataformaAutogestion.Infrastructure.Data;

#nullable disable

namespace PlataformaAutogestion.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261103183000_AddCompanyHistoryAccess")]
    public partial class AddCompanyHistoryAccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HistoryAccessEnabled",
                table: "Companys",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "HistoryAccessEnabled", table: "Companys");
        }
    }
}
