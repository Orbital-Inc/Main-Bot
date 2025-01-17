﻿using Discord.Interactions;

using MainBot.Database;

using Microsoft.EntityFrameworkCore;

namespace MainBot.Buttons;
public class CustomRoleButton : InteractionModuleBase<ShardedInteractionContext>
{
    [ComponentInteraction("custom-role-button")]
    public async Task ExecuteAsync()
    {
        await Context.Interaction.DeferAsync(true);
        Discord.WebSocket.SocketGuildUser? user = Context.Guild.GetUser(Context.Interaction.User.Id);
        if (user is null)
        {
            return;
        }

        await using var database = new DatabaseContext();
        Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            return;
        }

        if (guildEntry.guildSettings.hiddenRoleId is null)
        {
            return;
        }

        await user.AddRoleAsync((ulong)guildEntry.guildSettings.hiddenRoleId);
    }
}
