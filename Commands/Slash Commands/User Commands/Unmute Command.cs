using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Main_Bot.Database;
using Main_Bot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Main_Bot.Commands.Slash_Commands.User_Commands;

internal class UnmuteCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("unmute", "Unmute a user")]
    public async Task UnmuteUserCommand(IUser user)
    {
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", 60, true);
            return;
        }
        var mutedUserEntry = await Services.AutoUnmuteUserService._muteUsers.ToAsyncEnumerable().FirstOrDefaultAsync(x => x.id == user.Id);
        if (mutedUserEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Unmute User", $"", 60, true);
            return;
        }
        Services.AutoUnmuteUserService._muteUsers.Remove(mutedUserEntry);
        var role = Context.Guild.GetRole(guildEntry.guildSettings.muteRoleId);
        if (role is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Role doesn't exist.", 60, true);
            return;
        }
        //remove mute role on user
        await Context.Guild.GetUser(user.Id).RemoveRoleAsync(role);
        await Context.ReplyWithEmbedAsync("Unmute User", $"Successfully unmuted {user.Mention}", 60);
    }
}
