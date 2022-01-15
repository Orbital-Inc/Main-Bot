using Discord.Interactions;
using MainBot.Database;
using MainBot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MainBot.Commands.SlashCommands;

[Utilities.Attributes.RequireDeveloper]
public class HiddenCommands : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("guilds", "Displays a list of guilds")]
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
        await Context.ReplyWithEmbedAsync("Server List", serverDetails, deleteTimer: 120);
    }

    [SlashCommand("rainbow-refresh", "Randomly sets rainbow role colour")]
    public async Task SwapRainbowRoleColour()
    {
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        if (guildEntry.guildSettings.rainbowRoleId is not null)
        {
            var role = Context.Guild.GetRole((ulong)guildEntry.guildSettings.rainbowRoleId);
            await role.ModifyAsync(x =>
            {
                x.Color = Utilities.Miscallenous.RandomDiscordColour();
            });
            await Context.Interaction.DeferAsync(true);
            return;
        }
        await Context.ReplyWithEmbedAsync("Error Occured", "Rainbow role is not set");
    }
}
