﻿using Discord;
using Discord.Interactions;
using MainBot.Database;
using MainBot.Utilities.Attributes;
using MainBot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MainBot.Commands.SlashCommands.UserCommands;

[RequireModerator]
public class UnmuteCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("unmute", "Unmute a user")]
    public async Task UnmuteUserCommand(IUser user)
    {
        await using var database = new DatabaseContext();
        Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        Models.MuteUserModel? mutedUserEntry = await Services.AutoUnmuteUserService._muteUsers.ToAsyncEnumerable().FirstOrDefaultAsync(x => x.id == user.Id);
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
        if (DiscordExtensions.IsCommandExecutorPermsHigher(Context.User, user, guildEntry))
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Please check your permissions then try again.", deleteTimer: 60);
            return;
        }
        Discord.WebSocket.SocketRole? role = Context.Guild.GetRole((ulong)guildEntry.guildSettings.muteRoleId);
        if (role is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Role doesn't exist.", deleteTimer: 60, invisible: true);
            return;
        }
        Services.AutoUnmuteUserService._muteUsers.Remove(mutedUserEntry);
        //remove mute role on user
        await Context.Guild.GetUser(user.Id).RemoveRoleAsync(role);
        await Context.ReplyWithEmbedAsync("Unmute", $"Successfully unmuted {user.Mention}", deleteTimer: 60);
    }
}
