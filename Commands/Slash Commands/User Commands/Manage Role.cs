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
    [SlashCommand("role", "Add/remove a role from a user.")]
    public async Task ExecuteCommand(IUser user, IRole role)
    {
        await using var database = new DatabaseContext();
        Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (DiscordExtensions.IsCommandExecutorPermsHigher(Context.User, user, guildEntry) is false)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Please check your permissions then try again.", deleteTimer: 60, invisible: true);
            return;
        }
        //role above user?
        int highestRole = 0;
        foreach (Discord.WebSocket.SocketRole? meRole in Context.Guild.CurrentUser.Roles)
        {
            if (meRole.Position > highestRole)
            {
                highestRole = meRole.Position;
            }
        }
        if (highestRole <= role.Position)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Please check your permissions then try again.", deleteTimer: 60, invisible: true);
            return;
        }
        Discord.Rest.RestGuildUser? guildUser = await Context.Client.Rest.GetGuildUserAsync(Context.Guild.Id, user.Id);
        if (guildUser is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occurred", "The user mentioned cannot be found, please try again", deleteTimer: 60, invisible: true);
            return;
        }
        if (guildUser.RoleIds.Contains(role.Id))
        {
            //remove da role
            await guildUser.RemoveRoleAsync(role.Id);
            await Context.ReplyWithEmbedAsync("Role Management", $"Successfully removed {role.Mention} role from {user.Mention}", deleteTimer: 120);
            return;
        }
        //add da role
        await guildUser.AddRoleAsync(role);
        await Context.ReplyWithEmbedAsync("Role Management", $"Successfully added {role.Mention} role to {user.Mention}", deleteTimer: 120);
    }
}
