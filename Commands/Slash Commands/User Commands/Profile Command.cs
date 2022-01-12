using Discord;
using Discord.Interactions;
using Main_Bot.Utilities.Extensions;

namespace Main_Bot.Commands.SlashCommands.UserCommands;

public class ProfileCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("profile", "Display details about your account.")]
    public async Task ViewProfile(IUser? user = null)
    {
        var userInfo = user is null ? Context.Guild.GetUser(Context.User.Id) : Context.Guild.GetUser(user.Id);
        await Context.ReplyWithEmbedAsync($"{userInfo.Username}'s Information", $"{userInfo.Mention} | {userInfo.Id}\n" +
            $"Creation Date: <t:{userInfo.CreatedAt.ToUnixTimeSeconds()}>\n" +
            $"Join Date: <t:{userInfo.JoinedAt.Value.ToUnixTimeSeconds()}>\n" +
            $"Boost Date: {(userInfo.PremiumSince is null ? "N/A" : $"<t:{userInfo.PremiumSince.Value.ToUnixTimeSeconds()}>")}\n" +
            $"Mute Status: {userInfo.IsMuted}", imageUrl: userInfo.GetAvatarUrl());
    }
}
