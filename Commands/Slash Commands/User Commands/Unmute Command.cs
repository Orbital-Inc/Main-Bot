using Discord;
using Discord.Interactions;
using Main_Bot.Database;
using Main_Bot.Utilities.Attributes;
using Main_Bot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Main_Bot.Commands.SlashCommands.UserCommands;

[RequireModerator]
public class UnmuteCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("unmute", "Unmute a user")]
    public async Task UnmuteUserCommand(IUser user)
    {
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        var mutedUserEntry = await Services.AutoUnmuteUserService._muteUsers.ToAsyncEnumerable().FirstOrDefaultAsync(x => x.id == user.Id);
        if (mutedUserEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Unmute User", $"User is not muted.", deleteTimer: 60, invisible: true);
            return;
        }
        if (guildEntry.guildSettings.muteRoleId is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Role doesn't exist.", deleteTimer: 60, invisible: true);
            return;
        }
        if (await user.GetUserPermissionLevel(guildEntry) >= await Context.User.GetUserPermissionLevel(guildEntry))
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Please check your permissions then try again.", deleteTimer: 60);
            return;
        }
        var role = Context.Guild.GetRole((ulong)guildEntry.guildSettings.muteRoleId);
        if (role is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Role doesn't exist.", deleteTimer: 60, invisible: true);
            return;
        }
        Services.AutoUnmuteUserService._muteUsers.Remove(mutedUserEntry);
        //remove mute role on user
        await Context.Guild.GetUser(user.Id).RemoveRoleAsync(role);
        await Context.ReplyWithEmbedAsync("Unmute User", $"Successfully unmuted {user.Mention}", deleteTimer: 60);
    }
}
