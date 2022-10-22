﻿using Discord.Interactions;

using MainBot.Utilities.Attributes;

namespace MainBot.Commands.SlashCommands;

[RequireModerator]
public class NukeCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("nuke", "Clear all messages in a channel.")]
    public async Task NukeChannelCommand() => await Services.DailyChannelNukeService.NukeChannelAsync(Context.Channel);
}
