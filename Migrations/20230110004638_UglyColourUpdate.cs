using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainBot.Migrations
{
    /// <inheritdoc />
    public partial class UglyColourUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long[]>(
                name: "uglyColours",
                table: "GuildSettings",
                type: "bigint[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "uglyColours",
                table: "GuildSettings");
        }
    }
}
