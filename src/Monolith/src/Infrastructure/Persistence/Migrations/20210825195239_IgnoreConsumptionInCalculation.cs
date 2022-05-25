using Microsoft.EntityFrameworkCore.Migrations;

namespace Nuyken.VeGasCo.Backend.Infrastructure.Persistence.Migrations
{
    public partial class IgnoreConsumptionInCalculation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IgnoreInCalculation",
                table: "Consumptions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IgnoreInCalculation",
                table: "Consumptions");
        }
    }
}
