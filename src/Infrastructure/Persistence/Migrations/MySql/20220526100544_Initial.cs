#nullable disable

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nuyken.Vegasco.Backend.Infrastructure.Persistence.Migrations.MySql;

public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "Cars",
                table => new
                {
                    Id = table.Column<Guid>("char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>("longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table => { table.PrimaryKey("PK_Cars", x => x.Id); })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
                "Consumptions",
                table => new
                {
                    Id = table.Column<Guid>("char(36)", nullable: false, collation: "ascii_general_ci"),
                    DateTime = table.Column<DateTime>("datetime(6)", nullable: false),
                    Distance = table.Column<double>("double", nullable: false),
                    Amount = table.Column<double>("double", nullable: false),
                    IgnoreInCalculation = table.Column<bool>("tinyint(1)", nullable: false),
                    CarId = table.Column<Guid>("char(36)", nullable: false, collation: "ascii_general_ci")
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
                })
            .Annotation("MySql:CharSet", "utf8mb4");

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