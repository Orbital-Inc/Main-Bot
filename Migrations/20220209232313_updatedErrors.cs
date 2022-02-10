using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainBot.Migrations;

public partial class updatedErrors : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "location",
            table: "Errors",
            newName: "stackTrace");

        migrationBuilder.AddColumn<string>(
            name: "extraInformation",
            table: "Errors",
            type: "longtext",
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<string>(
            name: "source",
            table: "Errors",
            type: "longtext",
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "extraInformation",
            table: "Errors");

        migrationBuilder.DropColumn(
            name: "source",
            table: "Errors");

        migrationBuilder.RenameColumn(
            name: "stackTrace",
            table: "Errors",
            newName: "location");
    }
}
