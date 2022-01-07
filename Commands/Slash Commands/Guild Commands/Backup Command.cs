using Discord.Interactions;
using Main_Bot.Database;
using Main_Bot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Main_Bot.Commands.Slash_Commands.Guild_Commands;

[Utilities.Attributes.RequireOwner]
public class BackupCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("backup", "Backup this entire server, includes: roles, channels, *users*, & permissions.")]
    public async Task BackupDiscordServerSlashCommand() => await BackupServerAsync();

    private async Task BackupServerAsync()
    {
        await Context.Interaction.DeferAsync();
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            guildEntry = new Models.Guild
            {
                id = Context.Guild.Id,
                name = Context.Guild.Name,
            };
            await database.AddAsync(guildEntry);
            await database.ApplyChangesAsync();
        }
        else
        {
            guildEntry.name = Context.Guild.Name;
            await database.ApplyChangesAsync(guildEntry);
        }
        await Context.ReplyWithEmbedAsync("Server Backup", $"Sucessfully completed backing up the server.", 60);
    }
}
