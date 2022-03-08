using Discord.Interactions;
using MainBot.Database;
using MainBot.Utilities.Attributes;
using MainBot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MainBot.Commands.SlashCommands.GuildCommands.SettingsCommands;

[RequireModerator]
public class DisplaySettingsCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("guild-settings", "Display guild settings")]
    public async Task ExecuteCommand()
    {
        await using var database = new DatabaseContext();
        Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        await Context.ReplyWithEmbedAsync("Guild Settings",
            $"Mute Role: {(guildEntry.guildSettings.muteRoleId is null ? "N/A" : $"<@&{guildEntry.guildSettings.muteRoleId}>")}\n" +
            $"Administrator Role: {(guildEntry.guildSettings.administratorRoleId is null ? "N/A" : $"<@&{guildEntry.guildSettings.administratorRoleId}>")}\n" +
            $"Moderator Role: {(guildEntry.guildSettings.moderatorRoleId is null ? "N/A" : $"<@&{guildEntry.guildSettings.moderatorRoleId}>")}\n" +
            $"Rainbow Role: {(guildEntry.guildSettings.rainbowRoleId is null ? "N/A" : $"<@&{guildEntry.guildSettings.rainbowRoleId}>")}\n" +
            $"Verify Role: {(guildEntry.guildSettings.verifyRoleId is null ? "N/A" : $"<@&{guildEntry.guildSettings.verifyRoleId}>")}\n" +
            $"Hidden Role: {(guildEntry.guildSettings.hiddenRoleId is null ? "N/A" : $"<@&{guildEntry.guildSettings.hiddenRoleId}>")}\n" +
            $"Message Log Channel: {(guildEntry.guildSettings.messageLogChannelId is null ? "N/A" : $"<#{guildEntry.guildSettings.messageLogChannelId}>")}\n" +
            $"User Log Channel: {(guildEntry.guildSettings.userLogChannelId is null ? "N/A" : $"<#{guildEntry.guildSettings.userLogChannelId}>")}", deleteTimer: 120, invisible: true);
    }
}
