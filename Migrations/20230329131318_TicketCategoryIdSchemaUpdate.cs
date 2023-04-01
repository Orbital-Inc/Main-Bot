using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainBot.Migrations;

/// <inheritdoc />
public partial class TicketCategoryIdSchemaUpdate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<decimal>(
            name: "ticketCategoryId",
            table: "GuildSettings",
            type: "numeric(20,0)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "ticketCategoryId",
            table: "GuildSettings");
    }
}
