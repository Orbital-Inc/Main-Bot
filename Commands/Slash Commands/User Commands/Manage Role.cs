using Discord;
using Discord.Interactions;
using MainBot.Database;
using MainBot.Utilities.Attributes;
using MainBot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MainBot.Commands.SlashCommands.UserCommands;

[RequireModerator]
public class ManageRoleCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("role", "Kick a user from the guild")]
    public async Task ExecuteCommand(IUser user, IRole role)
    {
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (await DiscordExtensions.IsCommandExecutorPermsHigher(Context.User, user, guildEntry))
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Please check your permissions then try again.", deleteTimer: 60);
            return;
        }
        //finish me later derp derp
    }
}
