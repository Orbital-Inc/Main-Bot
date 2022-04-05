using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainBot.Migrations;

/// <inheritdoc />
public partial class mutedUserBackup : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "MutedUsers",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                muteExpiryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                guildId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                muteRoleId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MutedUsers", x => x.key);
            })
            .Annotation("MySql:CharSet", "utf8mb4");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "MutedUsers");
    }
}
