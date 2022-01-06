using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Main_Bot.Database;
using Main_Bot.Utilities.Extensions;

namespace Main_Bot.Commands.Slash_Commands.Guild_Commands;

internal class GuildSettingsCommands : InteractionModuleBase<ShardedInteractionContext>
{
    public enum GuildRoleOption
    {
        set_mute_role,
        set_verify_role,
    }
    [SlashCommand("guild-role-settings", "Guild settings that involve setting a role.")]
    public async Task IdkYet(GuildRoleOption roleOption, IRole role)
    {
        switch(roleOption)
        {
            case GuildRoleOption.set_mute_role:
                await using (var database = new DatabaseContext()
                {

                })
                break;
            default:
                await Context.ReplyWithEmbedAsync("Error Occured", "", 60, true); 
                break;
        }
    }
}
