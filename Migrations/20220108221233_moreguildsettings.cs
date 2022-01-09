using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main_Bot.Migrations;

#pragma warning disable IDE1006 // Naming Styles
public partial class moreguildsettings : Migration
#pragma warning restore IDE1006 // Naming Styles
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<ulong>(
            name: "messageLogChannelId",
            table: "GuildSettings",
            type: "bigint unsigned",
            nullable: true);

        migrationBuilder.AddColumn<ulong>(
            name: "userLogChannelId",
            table: "GuildSettings",
            type: "bigint unsigned",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "messageLogChannelId",
            table: "GuildSettings");

        migrationBuilder.DropColumn(
            name: "userLogChannelId",
            table: "GuildSettings");
    }
}
