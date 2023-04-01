using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainBot.Migrations;

/// <inheritdoc />
public partial class InitialCreation : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.CreateTable(
            name: "Errors",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                errorTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                source = table.Column<string>(type: "text", nullable: true),
                message = table.Column<string>(type: "text", nullable: true),
                stackTrace = table.Column<string>(type: "text", nullable: true),
                extraInformation = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Errors", x => x.key);
            });

        _ = migrationBuilder.CreateTable(
            name: "GuildSettings",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                verifyRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                muteRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                rainbowRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                administratorRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                moderatorRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                hiddenRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                userLogChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                messageLogChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                systemLogChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: true)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_GuildSettings", x => x.key);
            });

        _ = migrationBuilder.CreateTable(
            name: "MutedUsers",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                muteExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                guildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                muteRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_MutedUsers", x => x.key);
            });

        _ = migrationBuilder.CreateTable(
            name: "NukeChannels",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: true),
                id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                guildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_NukeChannels", x => x.key);
            });

        _ = migrationBuilder.CreateTable(
            name: "Guilds",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                guildSettingskey = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Guilds", x => x.key);
                _ = table.ForeignKey(
                    name: "FK_Guilds_GuildSettings_guildSettingskey",
                    column: x => x.guildSettingskey,
                    principalTable: "GuildSettings",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_Guilds_guildSettingskey",
            table: "Guilds",
            column: "guildSettingskey");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "Errors");

        _ = migrationBuilder.DropTable(
            name: "Guilds");

        _ = migrationBuilder.DropTable(
            name: "MutedUsers");

        _ = migrationBuilder.DropTable(
            name: "NukeChannels");

        _ = migrationBuilder.DropTable(
            name: "GuildSettings");
    }
}
