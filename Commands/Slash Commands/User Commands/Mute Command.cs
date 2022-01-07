using Discord;
using Discord.Interactions;
using Main_Bot.Database;
using Main_Bot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Main_Bot.Commands.Slash_Commands.User_Commands;

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
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", 60, true);
            return;
        }
        var mutedUserEntry = await Services.AutoUnmuteUserService._muteUsers.ToAsyncEnumerable().FirstOrDefaultAsync(x => x.id == user.Id);
        if (mutedUserEntry is not null)
        {
            await Context.ReplyWithEmbedAsync("Mute User", $"Failed to mute {user.Mention}, user is already muted.", 60, true);
            return;
        }
        switch (durationOptions)
        {
            case muteDurationOptions.minutes:
                break;
            default:
                await Context.ReplyWithEmbedAsync("", "", 60, true);
                return;
        }
        //get mute role
        var role = Context.Guild.GetRole(guildEntry.guildSettings.muteRoleId);
        if (role is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Role doesn't exist.", 60, true);
            return;
        }
        mutedUserEntry = new Models.MuteUserModel
        {
            id = user.Id,
            guildId = Context.Guild.Id,
            muteRoleId = role.Id,
            muteExpiryDate = DateTime.Now.AddMinutes(duration),
        };
        Services.AutoUnmuteUserService._muteUsers.Add(mutedUserEntry);
        //set mute role on user
        await Context.Guild.GetUser(user.Id).AddRoleAsync(role);
        await Context.ReplyWithEmbedAsync("Mute User", $"Successfully muted {user.Mention} until {mutedUserEntry.muteExpiryDate}", 60);
    }
}
