using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main_Bot.Migrations;

public partial class somenewshit : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<ulong>(
            name: "administratorRoleId",
            table: "GuildSettings",
            type: "bigint unsigned",
            nullable: true);

        migrationBuilder.AddColumn<ulong>(
            name: "hiddenRoleId",
            table: "GuildSettings",
            type: "bigint unsigned",
            nullable: true);

        migrationBuilder.AddColumn<ulong>(
            name: "moderatorRoleId",
            table: "GuildSettings",
            type: "bigint unsigned",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "administratorRoleId",
            table: "GuildSettings");

        migrationBuilder.DropColumn(
            name: "hiddenRoleId",
            table: "GuildSettings");

        migrationBuilder.DropColumn(
            name: "moderatorRoleId",
            table: "GuildSettings");
    }
}
