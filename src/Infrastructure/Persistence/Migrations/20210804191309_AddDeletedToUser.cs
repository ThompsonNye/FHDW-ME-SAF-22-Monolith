using Microsoft.EntityFrameworkCore.Migrations;

namespace Nuyken.VeGasCo.Backend.Infrastructure.Persistence.Migrations
{
    public partial class AddDeletedToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Cars",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Cars");
        }
    }
}
