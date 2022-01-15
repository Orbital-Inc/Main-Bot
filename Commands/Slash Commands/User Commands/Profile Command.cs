using Discord;
using Discord.Interactions;
using MainBot.Utilities.Extensions;

namespace MainBot.Commands.SlashCommands.UserCommands;

public class ProfileCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("profile", "Display details about your account.")]
    public async Task ViewProfile(IUser? user = null)
    {
        var userInfo = user is null ? Context.Guild.GetUser(Context.User.Id) : Context.Guild.GetUser(user.Id);
        await Context.ReplyWithEmbedAsync($"{userInfo.Username}'s Information", $"{userInfo.Mention} | {userInfo.Id}\n" +
            $"Creation Date: <t:{userInfo.CreatedAt.ToUnixTimeSeconds()}>\n" +
            $"Join Date: {(userInfo.JoinedAt is null ? "N/A" : $"<t:{userInfo.JoinedAt.Value.ToUnixTimeSeconds()}>")}\n" +
            $"Boost Date: {(userInfo.PremiumSince is null ? "N/A" : $"<t:{userInfo.PremiumSince.Value.ToUnixTimeSeconds()}>")}\n" +
            $"Mute Status: {Services.AutoUnmuteUserService._muteUsers.FirstOrDefault(x => x.id == userInfo.Id) is not null}", imageUrl: userInfo.GetAvatarUrl()).ConfigureAwait(false);
    }
}
