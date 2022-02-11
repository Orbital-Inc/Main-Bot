﻿using System.Reflection;
using Discord.Interactions;
using MainBot.Utilities.Extensions;

namespace MainBot.Commands.SlashCommands;

public class InformationCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("information", "Display information about the bot")]
    public async Task ExecuteCommand()
    {
        var appInfo = await Context.Client.GetApplicationInfoAsync();
        await Context.ReplyWithEmbedAsync("Information",
            $"Guild Count: {Context.Client.Guilds.Count}\n" +
            $"Developer: {(Context.Guild.GetUser(appInfo.Owner.Id) is null ? $"{appInfo.Owner.Username}#{appInfo.Owner.Discriminator}" : appInfo.Owner.Mention)}\n" +
            $"Uptime: N/A\n" +
            $"Build Version: {Assembly.GetExecutingAssembly().GetName().Version}\n" +
            $"Description: {appInfo.Description}\n" +
            $"Terms of Service: {appInfo.TermsOfService}"
            );
        //fix uptime
    }
}