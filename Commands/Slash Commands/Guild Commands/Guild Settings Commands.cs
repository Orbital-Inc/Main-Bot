using Discord;
using Discord.Interactions;
using Main_Bot.Database;
using Main_Bot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Main_Bot.Commands.Slash_Commands.Guild_Commands;

internal class GuildSettingsCommands : InteractionModuleBase<ShardedInteractionContext>
{
    public enum guildRoleOption
    {
        set_mute_role,
        set_verify_role,
    }

    [SlashCommand("guild-role-settings", "Guild settings that involve setting a role.")]
    public async Task IdkYet(guildRoleOption roleOption, IRole role)
    {
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", 60, true);
            return;
        }
        switch (roleOption)
        {
            case guildRoleOption.set_mute_role:
                guildEntry.guildSettings.muteRoleId = role.Id;
                await database.ApplyChangesAsync(guildEntry);
                await Context.ReplyWithEmbedAsync("Mute Role", $"Successfully set {role.Mention} as the mute role.", 60, true);
                break;
            case guildRoleOption.set_verify_role:
                guildEntry.guildSettings.verifyRoleId = role.Id;
                await database.ApplyChangesAsync(guildEntry);
                await Context.ReplyWithEmbedAsync("Verify Role", $"Successfully set {role.Mention} as the verify role.", 60, true);
                break;
            default:
                await Context.ReplyWithEmbedAsync("Error Occured", "Invalid option selected.", 60, true);
                break;
        }
    }


}
