using Discord;
using Discord.Interactions;

using MainBot.Database;
using MainBot.Utilities.Attributes;
using MainBot.Utilities.Extensions;

using Microsoft.EntityFrameworkCore;

namespace MainBot.Commands.SlashCommands.UserCommands;

[RequireModerator]
public class KickCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("kick", "Kick a user from the guild.")]
    public async Task ExecuteCommand(IUser user)
    {
        await using var database = new DatabaseContext();
        Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (DiscordExtensions.IsCommandExecutorPermsHigher(Context.User, user, guildEntry) is false)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Please check your permissions then try again.", deleteTimer: 60, invisible: true);
            return;
        }
        await Context.Guild.GetUser(user.Id).KickAsync();
        await Context.ReplyWithEmbedAsync("Kick", $"Beamed {user.Mention} lawl", deleteTimer: 240);
    }
}
