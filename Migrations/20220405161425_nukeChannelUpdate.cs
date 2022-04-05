using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainBot.Migrations;

/// <inheritdoc />
public partial class nukeChannelUpdate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GuildUsers");

        migrationBuilder.DropTable(
            name: "UserRoles");

        migrationBuilder.DropTable(
            name: "Role");

        migrationBuilder.DropTable(
            name: "Users");

        migrationBuilder.DropTable(
            name: "RolePermissions");

        migrationBuilder.CreateTable(
            name: "NukeChannels",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                name = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                guildId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NukeChannels", x => x.key);
            })
            .Annotation("MySql:CharSet", "utf8mb4");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "NukeChannels");

        migrationBuilder.CreateTable(
            name: "RolePermissions",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                AddReactions = table.Column<bool>(type: "tinyint(1)", nullable: false),
                Administrator = table.Column<bool>(type: "tinyint(1)", nullable: false),
                AttachFiles = table.Column<bool>(type: "tinyint(1)", nullable: false),
                BanMembers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ChangeNickname = table.Column<bool>(type: "tinyint(1)", nullable: false),
                Connect = table.Column<bool>(type: "tinyint(1)", nullable: false),
                CreateInstantInvite = table.Column<bool>(type: "tinyint(1)", nullable: false),
                CreatePrivateThreads = table.Column<bool>(type: "tinyint(1)", nullable: false),
                CreatePublicThreads = table.Column<bool>(type: "tinyint(1)", nullable: false),
                DeafenMembers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                EmbedLinks = table.Column<bool>(type: "tinyint(1)", nullable: false),
                KickMembers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageChannels = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageEmojisAndStickers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageEvents = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageGuild = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageMessages = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageNicknames = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageRoles = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageThreads = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageWebhooks = table.Column<bool>(type: "tinyint(1)", nullable: false),
                MentionEveryone = table.Column<bool>(type: "tinyint(1)", nullable: false),
                MoveMembers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                MuteMembers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                PrioritySpeaker = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ReadMessageHistory = table.Column<bool>(type: "tinyint(1)", nullable: false),
                RequestToSpeak = table.Column<bool>(type: "tinyint(1)", nullable: false),
                SendMessages = table.Column<bool>(type: "tinyint(1)", nullable: false),
                SendMessagesInThreads = table.Column<bool>(type: "tinyint(1)", nullable: false),
                SendTTSMessages = table.Column<bool>(type: "tinyint(1)", nullable: false),
                Speak = table.Column<bool>(type: "tinyint(1)", nullable: false),
                StartEmbeddedActivities = table.Column<bool>(type: "tinyint(1)", nullable: false),
                Stream = table.Column<bool>(type: "tinyint(1)", nullable: false),
                UseApplicationCommands = table.Column<bool>(type: "tinyint(1)", nullable: false),
                UseExternalEmojis = table.Column<bool>(type: "tinyint(1)", nullable: false),
                UseExternalStickers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                UseVAD = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ViewAuditLog = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ViewChannel = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ViewGuildInsights = table.Column<bool>(type: "tinyint(1)", nullable: false),
                moderateMembers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                useVoiceActivation = table.Column<bool>(type: "tinyint(1)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RolePermissions", x => x.key);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                accessToken = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                refreshToken = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                username = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.key);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Role",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                permissionskey = table.Column<int>(type: "int", nullable: false),
                color = table.Column<uint>(type: "int unsigned", nullable: false),
                icon = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                isEveryone = table.Column<bool>(type: "tinyint(1)", nullable: false),
                isHoisted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                isManaged = table.Column<bool>(type: "tinyint(1)", nullable: false),
                isMentionable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                name = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                position = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Role", x => x.key);
                table.ForeignKey(
                    name: "FK_Role_RolePermissions_permissionskey",
                    column: x => x.permissionskey,
                    principalTable: "RolePermissions",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "GuildUsers",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                userkey = table.Column<int>(type: "int", nullable: false),
                Guildkey = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GuildUsers", x => x.key);
                table.ForeignKey(
                    name: "FK_GuildUsers_Guilds_Guildkey",
                    column: x => x.Guildkey,
                    principalTable: "Guilds",
                    principalColumn: "key");
                table.ForeignKey(
                    name: "FK_GuildUsers_Users_userkey",
                    column: x => x.userkey,
                    principalTable: "Users",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "UserRoles",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                guildkey = table.Column<int>(type: "int", nullable: false),
                rolekey = table.Column<int>(type: "int", nullable: false),
                Userkey = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserRoles", x => x.key);
                table.ForeignKey(
                    name: "FK_UserRoles_Guilds_guildkey",
                    column: x => x.guildkey,
                    principalTable: "Guilds",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UserRoles_Role_rolekey",
                    column: x => x.rolekey,
                    principalTable: "Role",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UserRoles_Users_Userkey",
                    column: x => x.Userkey,
                    principalTable: "Users",
                    principalColumn: "key");
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "IX_GuildUsers_Guildkey",
            table: "GuildUsers",
            column: "Guildkey");

        migrationBuilder.CreateIndex(
            name: "IX_GuildUsers_userkey",
            table: "GuildUsers",
            column: "userkey");

        migrationBuilder.CreateIndex(
            name: "IX_Role_permissionskey",
            table: "Role",
            column: "permissionskey");

        migrationBuilder.CreateIndex(
            name: "IX_UserRoles_guildkey",
            table: "UserRoles",
            column: "guildkey");

        migrationBuilder.CreateIndex(
            name: "IX_UserRoles_rolekey",
            table: "UserRoles",
            column: "rolekey");

        migrationBuilder.CreateIndex(
            name: "IX_UserRoles_Userkey",
            table: "UserRoles",
            column: "Userkey");
    }
}
