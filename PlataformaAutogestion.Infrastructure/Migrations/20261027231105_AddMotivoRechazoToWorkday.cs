using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlataformaAutogestion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMotivoRechazoToWorkday : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazo",
                table: "Workdays",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MotivoRechazo",
                table: "Workdays");
        }
    }
}
