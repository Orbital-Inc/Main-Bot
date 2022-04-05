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
    public async Task MuteUserCommand(IUser user, muteDurationOptions durationOptions, int duration)
    {
        await using var database = new DatabaseContext();
        Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        Database.Models.MuteUser? mutedUserEntry = await database.MutedUsers.FirstOrDefaultAsync(x => x.id == user.Id && x.guildId == Context.Guild.Id);
        if (mutedUserEntry is not null)
        {
            await Context.ReplyWithEmbedAsync("Mute User", $"Failed to mute {user.Mention}, user is already muted.", deleteTimer: 60, invisible: true);
            return;
        }
        if (guildEntry.guildSettings.muteRoleId is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Role doesn't exist.", deleteTimer: 60, invisible: true);
            return;
        }
        if (DiscordExtensions.IsCommandExecutorPermsHigher(Context.User, user, guildEntry))
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Please check your permissions then try again.", deleteTimer: 60);
            return;
        }
        DateTime muteTime = DateTime.Now;
        switch (durationOptions)
        {
            case muteDurationOptions.minutes:
                muteTime = DateTime.Now.AddMinutes(duration);
                break;
            case muteDurationOptions.seconds:
                muteTime = DateTime.Now.AddSeconds(duration);
                break;
            case muteDurationOptions.hours:
                muteTime = DateTime.Now.AddHours(duration);
                break;
            case muteDurationOptions.days:
                muteTime = DateTime.Now.AddDays(duration);
                break;
            default:
                await Context.ReplyWithEmbedAsync("Error Occured", "Invalid option selected.", deleteTimer: 60, invisible: true);
                return;
        }
        //get mute role
        Discord.WebSocket.SocketRole? role = Context.Guild.GetRole((ulong)guildEntry.guildSettings.muteRoleId);
        if (role is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Role doesn't exist.", deleteTimer: 60, invisible: true);
            return;
        }
        mutedUserEntry = new Database.Models.MuteUser
        {
            id = user.Id,
            guildId = Context.Guild.Id,
            muteRoleId = role.Id,
            muteExpiryDate = muteTime,
        };
        await database.MutedUsers.AddAsync(mutedUserEntry);
        await database.ApplyChangesAsync();
        //set mute role on user
        await Context.Guild.GetUser(user.Id).AddRoleAsync(role);
        DateTimeOffset yeet = mutedUserEntry.muteExpiryDate;
        await Context.ReplyWithEmbedAsync("Mute", $"Successfully muted {user.Mention} until <t:{yeet.ToUnixTimeSeconds()}>", deleteTimer: 60);
    }
}
