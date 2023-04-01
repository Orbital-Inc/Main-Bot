using Discord;
using Discord.Interactions;

using MainBot.Database;
using MainBot.Utilities.Attributes;
using MainBot.Utilities.Extensions;

using Microsoft.EntityFrameworkCore;

namespace MainBot.Commands.SlashCommands.UserCommands;

[RequireModerator]
public class MuteCommand : InteractionModuleBase<ShardedInteractionContext>
{
    public enum muteDurationOptions
    {
        seconds,
        minutes,
        hours,
        days
    }

    [SlashCommand("mute", "Mutes a user for a specific amount of time.")]
    public async Task MuteUserCommand(IUser user, muteDurationOptions durationOptions, int duration, string? reason = "N/A")
    {
        await using var database = new DatabaseContext();
        Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        Database.Models.MuteUser? mutedUserEntry = await database.MutedUsers.FirstOrDefaultAsync(x => x.id == user.Id && x.guildId == Context.Guild.Id);
        if (mutedUserEntry is not null)
        {
            _ = await Context.ReplyWithEmbedAsync("Mute User", $"Failed to mute {user.Mention}, user is already muted.", deleteTimer: 60, invisible: true);
            return;
        }
        if (guildEntry.guildSettings.muteRoleId is null)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "Role doesn't exist.", deleteTimer: 60, invisible: true);
            return;
        }
        if (DiscordExtensions.IsCommandExecutorPermsHigher(Context.User, user, guildEntry) is false)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "Please check your permissions then try again.", deleteTimer: 60, invisible: true);
            return;
        }
        DateTime muteTime = DateTime.UtcNow;
        switch (durationOptions)
        {
            case muteDurationOptions.minutes:
                muteTime = DateTime.UtcNow.AddMinutes(duration);
                break;
            case muteDurationOptions.seconds:
                muteTime = DateTime.UtcNow.AddSeconds(duration);
                break;
            case muteDurationOptions.hours:
                muteTime = DateTime.UtcNow.AddHours(duration);
                break;
            case muteDurationOptions.days:
                muteTime = DateTime.UtcNow.AddDays(duration);
                break;
            default:
                _ = await Context.ReplyWithEmbedAsync("Error Occured", "Invalid option selected.", deleteTimer: 60, invisible: true);
                return;
        }
        //get mute role
        Discord.WebSocket.SocketRole? role = Context.Guild.GetRole((ulong)guildEntry.guildSettings.muteRoleId);
        if (role is null)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "Role doesn't exist.", deleteTimer: 60, invisible: true);
            return;
        }
        mutedUserEntry = new Database.Models.MuteUser
        {
            id = user.Id,
            guildId = Context.Guild.Id,
            muteRoleId = role.Id,
            muteExpiryDate = muteTime,
        };
        _ = await database.MutedUsers.AddAsync(mutedUserEntry);
        await database.ApplyChangesAsync();
        //set mute role on user
        await Context.Guild.GetUser(user.Id).AddRoleAsync(role);
        DateTimeOffset mutedUntilDateTime = mutedUserEntry.muteExpiryDate;
        _ = await Context.ReplyWithEmbedAsync("Mute", $"Successfully muted {user.Mention} until <t:{mutedUntilDateTime.ToUnixTimeSeconds()}>");
        if (guildEntry.guildSettings.userLogChannelId is null)
        {
            return;
        }

        var logChannel = Context.Guild.GetChannel((ulong)guildEntry.guildSettings.userLogChannelId);
        if (logChannel is not null)
        {
            _ = await logChannel.SendEmbedAsync("Muted User", $"User: {user.Username}#{user.Discriminator} - {user.Mention}\nMuted Until: <t:{mutedUntilDateTime.ToUnixTimeSeconds()}>\nMute Reason: {reason}\nMuted By: {Context.Interaction.User.Mention}", $"{user.Id}", user.GetAvatarUrl());
        }
    }
}
