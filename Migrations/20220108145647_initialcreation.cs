using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainBot.Migrations;
#pragma warning disable IDE1006 // Naming Styles
public partial class initialcreation : Migration
#pragma warning restore IDE1006 // Naming Styles
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Errors",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                errorTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                location = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                message = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Errors", x => x.key);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "GuildSettings",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                verifyRoleId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                muteRoleId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                rainbowRoleId = table.Column<ulong>(type: "bigint unsigned", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GuildSettings", x => x.key);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                username = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                accessToken = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                refreshToken = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.key);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Guilds",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                name = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                guildSettingskey = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Guilds", x => x.key);
                table.ForeignKey(
                    name: "FK_Guilds_GuildSettings_guildSettingskey",
                    column: x => x.guildSettingskey,
                    principalTable: "GuildSettings",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "GuildUser",
            columns: table => new
            {
                guildskey = table.Column<int>(type: "int", nullable: false),
                userskey = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GuildUser", x => new { x.guildskey, x.userskey });
                table.ForeignKey(
                    name: "FK_GuildUser_Guilds_guildskey",
                    column: x => x.guildskey,
                    principalTable: "Guilds",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GuildUser_Users_userskey",
                    column: x => x.userskey,
                    principalTable: "Users",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "IX_Guilds_guildSettingskey",
            table: "Guilds",
            column: "guildSettingskey");

        migrationBuilder.CreateIndex(
            name: "IX_GuildUser_userskey",
            table: "GuildUser",
            column: "userskey");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Errors");

        migrationBuilder.DropTable(
            name: "GuildUser");

        migrationBuilder.DropTable(
            name: "Guilds");

        migrationBuilder.DropTable(
            name: "Users");

        migrationBuilder.DropTable(
            name: "GuildSettings");
    }
}
