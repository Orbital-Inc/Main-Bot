using Discord;
using Discord.Interactions;
using Main_Bot.Database;
using Main_Bot.Utilities.Attributes;
using Main_Bot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Main_Bot.Commands.SlashCommands.GuildCommands.SettingsCommands;

[RequireAdministrator]
public class GuildRoleSettingsCommand : InteractionModuleBase<ShardedInteractionContext>
{
    public enum guildRoleOption
    {
        set_mute_role,
        set_verify_role,
        set_rainbow_role,
        set_hidden_role,
        set_administrator_role,
        set_moderator_role
    }

    [SlashCommand("guild-role-settings", "Guild settings that involve setting a role.")]
    public async Task ExecuteCommand(guildRoleOption roleOption, IRole role)
    {
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        switch (roleOption)
        {
            case guildRoleOption.set_mute_role:
                guildEntry.guildSettings.muteRoleId = role.Id;
                //set correct perms on all channels that exist if they dont already
                break;
            case guildRoleOption.set_verify_role:
                guildEntry.guildSettings.verifyRoleId = role.Id;
                break;
            case guildRoleOption.set_rainbow_role:
                guildEntry.guildSettings.rainbowRoleId = role.Id;
                Services.RainbowRoleService._rainbowRoleGuilds.Add(new Models.RainbowRoleModel
                {
                    roleId = role.Id,
                    guildId = Context.Guild.Id,
                });
                break;
            case guildRoleOption.set_moderator_role:
                guildEntry.guildSettings.moderatorRoleId = role.Id;
                break;
            case guildRoleOption.set_administrator_role:
                guildEntry.guildSettings.administratorRoleId = role.Id;
                break;
            case guildRoleOption.set_hidden_role:
                guildEntry.guildSettings.hiddenRoleId = role.Id;
                break;
            default:
                await Context.ReplyWithEmbedAsync("Error Occured", "Invalid option selected.", deleteTimer: 60, invisible: true);
                return;
        }
        await database.ApplyChangesAsync(guildEntry);
        await Context.ReplyWithEmbedAsync("Guild Role Settings", $"Successfully set the role to: {role.Mention}", deleteTimer: 60, invisible: true);
    }
}
