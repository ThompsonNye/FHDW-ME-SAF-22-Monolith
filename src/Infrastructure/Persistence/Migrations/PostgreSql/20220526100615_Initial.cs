#nullable disable

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nuyken.Vegasco.Backend.Infrastructure.Persistence.Migrations.PostgreSql;

public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "Cars",
            table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                Name = table.Column<string>("text", nullable: true)
            },
            constraints: table => { table.PrimaryKey("PK_Cars", x => x.Id); });

        migrationBuilder.CreateTable(
            "Consumptions",
            table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                DateTime = table.Column<DateTime>("timestamp with time zone", nullable: false),
                Distance = table.Column<double>("double precision", nullable: false),
                Amount = table.Column<double>("double precision", nullable: false),
                IgnoreInCalculation = table.Column<bool>("boolean", nullable: false),
                CarId = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Consumptions", x => x.Id);
                table.ForeignKey(
                    "FK_Consumptions_Cars_CarId",
                    x => x.CarId,
                    "Cars",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "IX_Consumptions_CarId",
            "Consumptions",
            "CarId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "Consumptions");

        migrationBuilder.DropTable(
            "Cars");
    }
}