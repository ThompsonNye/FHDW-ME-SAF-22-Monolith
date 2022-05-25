using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Nuyken.VeGasCo.Backend.Infrastructure.Persistence.Migrations
{
    public partial class UpdateConsumption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Consumptions",
                newName: "DateTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "Consumptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<double>(
                name: "Distance",
                table: "Consumptions",
                type: "double",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Consumptions",
                type: "double",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "Consumptions",
                newName: "Date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Consumptions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Distance",
                table: "Consumptions",
                type: "double",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Consumptions",
                type: "double",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double");
        }
    }
}
