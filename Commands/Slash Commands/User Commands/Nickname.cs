using Discord;
using Discord.Interactions;

using MainBot.Database;
using MainBot.Utilities.Attributes;
using MainBot.Utilities.Extensions;

using Microsoft.EntityFrameworkCore;

namespace MainBot.Commands.SlashCommands.UserCommands;

[RequireModerator]
public class NicknameCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("nickname", "Change a user's nickname or reset it.")]
    public async Task ExecuteCommand(IUser user, string? nickname = null)
    {
        await using var database = new DatabaseContext();
        Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (DiscordExtensions.IsCommandExecutorPermsHigher(Context.User, user, guildEntry) is false)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Please check your permissions then try again.", deleteTimer: 60, invisible: true);
            return;
        }
        if (string.IsNullOrWhiteSpace(nickname))
        {
            await Context.Guild.GetUser(user.Id).ModifyAsync(x => x.Nickname = null);
            await Context.ReplyWithEmbedAsync("Nickname", $"Successfully removed {user.Mention}'s nickname.", deleteTimer: 120);
            return;
        }
        await Context.Guild.GetUser(user.Id).ModifyAsync(x => x.Nickname = nickname);
        await Context.ReplyWithEmbedAsync("Nickname", $"Successfully set {user.Mention}'s nickname to {nickname}.", deleteTimer: 120);
    }
}
