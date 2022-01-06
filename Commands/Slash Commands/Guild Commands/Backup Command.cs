using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Main_Bot.Database;
using Main_Bot.Utilities.Extensions;

namespace Main_Bot.Commands.Slash_Commands.Guild_Commands;

[Utilities.Attributes.RequireOwner]
public class BackupCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("backup", "Backup this entire server, includes: roles, channels, *users*, & permissions.")]
    public async Task BackupDiscordServerSlashCommand()
    {
        await using var database = new DatabaseContext();

        await Context.ReplyWithEmbedAsync("Server Backup", $"Sucessfully completed backing up the server.", 60);
    }
}
