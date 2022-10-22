using Discord.Interactions;

using MainBot.Database;

using Microsoft.EntityFrameworkCore;

namespace MainBot.Buttons;

public class VerificationButton : InteractionModuleBase<ShardedInteractionContext>
{
    [ComponentInteraction("verify-button")]
    public async Task VerifyUserTask()
    {
        await Context.Interaction.DeferAsync(true);
        Discord.WebSocket.SocketGuildUser? user = Context.Guild.GetUser(Context.Interaction.User.Id);
        if (user is null)
            return;
        await using var database = new DatabaseContext();
        Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
            return;
        if (guildEntry.guildSettings.verifyRoleId is null)
            return;
        await user.AddRoleAsync((ulong)guildEntry.guildSettings.verifyRoleId);
    }
}
