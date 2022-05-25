using Microsoft.EntityFrameworkCore.Migrations;

namespace Nuyken.VeGasCo.Backend.Infrastructure.Persistence.Migrations
{
    public partial class PendingChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarColorOverride",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Consumptions");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Cars");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CarColorOverride",
                table: "Settings",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Settings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Consumptions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Cars",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Cars",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
