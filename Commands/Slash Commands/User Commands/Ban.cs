﻿using Discord;
using Discord.Interactions;

using MainBot.Database;
using MainBot.Utilities.Attributes;
using MainBot.Utilities.Extensions;

using Microsoft.EntityFrameworkCore;

namespace MainBot.Commands.SlashCommands.UserCommands;

[RequireModerator]
public class BanCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("ban", "Ban a user from the guild.")]
    public async Task ExecuteCommand(IUser user, string? reason = null, int pruneDays = 7)
    {
        await using var database = new DatabaseContext();
        Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (DiscordExtensions.IsCommandExecutorPermsHigher(Context.User, user, guildEntry) is false)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "Please check your permissions then try again.", deleteTimer: 60, invisible: true);
            return;
        }
        await Context.Guild.GetUser(user.Id).BanAsync(pruneDays, reason);
        _ = await Context.ReplyWithEmbedAsync("Ban", $"Beamed {user.Mention} lawl", deleteTimer: 240);
        if (guildEntry is null)
        {
            return;
        }

        if (guildEntry.guildSettings.userLogChannelId is null)
        {
            return;
        }

        var logChannel = Context.Guild.GetChannel((ulong)guildEntry.guildSettings.userLogChannelId);
        if (logChannel is not null)
        {
            _ = await logChannel.SendEmbedAsync("Banned User", $"User: {user.Username}#{user.Discriminator} - {user.Mention}\nReason: {(string.IsNullOrWhiteSpace(reason) ? "N/A" : reason)}\nBanned By: {Context.Interaction.User.Mention}", $"{user.Id}", user.GetAvatarUrl());
        }
    }
}
