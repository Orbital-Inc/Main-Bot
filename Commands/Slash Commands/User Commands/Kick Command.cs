using Discord;
using Discord.Interactions;
using Main_Bot.Database;
using Main_Bot.Utilities.Attributes;
using Main_Bot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Main_Bot.Commands.SlashCommands.UserCommands;

[RequireModerator]
public class KickCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("kick", "Kick a user from the guild")]
    public async Task ExecuteCommand(IUser user)
    {
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (await user.GetUserPermissionLevel(guildEntry) >= await Context.User.GetUserPermissionLevel(guildEntry))
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Please check your permissions then try again.", deleteTimer: 60);
            return;
        }
        await Context.Guild.GetUser(user.Id).KickAsync();
        await Context.ReplyWithEmbedAsync("Kick User", $"Beamed {user.Mention} lawl");
    }
}
