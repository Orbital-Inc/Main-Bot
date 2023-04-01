﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainBot.Migrations;

/// <inheritdoc />
public partial class CommandLogChannel : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<decimal>(
            name: "commandLogChannelId",
            table: "GuildSettings",
            type: "numeric(20,0)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "commandLogChannelId",
            table: "GuildSettings");
    }
}
