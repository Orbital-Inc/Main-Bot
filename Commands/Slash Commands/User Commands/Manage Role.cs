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
    //[SlashCommand("role", "Add/remove a role from a user")]
    public async Task ExecuteCommand(IUser user, IRole role)
    {
        await using var database = new DatabaseContext();
        Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (await DiscordExtensions.IsCommandExecutorPermsHigher(Context.User, user, guildEntry))
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Please check your permissions then try again.", deleteTimer: 60);
            return;
        }
        Discord.Rest.RestGuildUser? guildUser = await Context.Client.Rest.GetGuildUserAsync(Context.Guild.Id, user.Id);
        if (guildUser.RoleIds.Contains(role.Id))
        {
            //remove da role
            await guildUser.RemoveRoleAsync(role.Id);
            await Context.ReplyWithEmbedAsync("Role Management", $"Successfully removed {role.Mention} role from {user.Mention}");
            return;
        }
        //add da role
        await guildUser.AddRoleAsync(role);
        await Context.ReplyWithEmbedAsync("Role Management", $"Successfully added {role.Mention} role to {user.Mention}");
    }
}
