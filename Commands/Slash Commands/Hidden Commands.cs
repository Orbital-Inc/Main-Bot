using Discord.Interactions;
using Main_Bot.Database;
using Main_Bot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Main_Bot.Commands.Slash_Commands;

[Utilities.Attributes.RequireDeveloper]
public class HiddenCommands : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("guilds", "Hidden command one")]
    public async Task ListDiscordServersCommand()
    {
        await Context.Interaction.DeferAsync();
        await using var database = new DatabaseContext();
        var serverDetails = string.Empty;
        await Context.Client.Guilds.ToAsyncEnumerable().ForEachAwaitAsync(async guild =>
        {
                serverDetails += await database.Guilds.FirstOrDefaultAsync(x => x.id == guild.Id) is not null ? 
                $"{guild.Name} | {guild.Id} | {guild.MemberCount} ~ {guild.Owner.Username}#{guild.Owner.Discriminator} | BACKED UP\n"
                : $"{guild.Name} | {guild.Id} | {guild.MemberCount} ~ {guild.Owner.Username}#{guild.Owner.Discriminator}\n";
        });
        await Context.ReplyWithEmbedAsync("Server List", serverDetails, 120);
    }


}
