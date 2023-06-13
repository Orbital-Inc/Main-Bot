using System.Diagnostics;
using System.Reflection;

using Discord.Interactions;

using MainBot.Utilities.Extensions;

namespace MainBot.Commands.SlashCommands;

public class InformationCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("information", "Display information about the bot.")]
    public async Task ExecuteCommand()
    {
        Discord.Rest.RestApplication? appInfo = await Context.Client.GetApplicationInfoAsync();
        _ = await Context.ReplyWithEmbedAsync("Information",
            $"Guild Count: {Context.Client.Guilds.Count}\n" +
            $"Guild Member Count: {(Context.Guild is null ? "N/A" : Context.Guild.MemberCount)}\n" +
            $"Developer: {(Context.Guild is null ? $"{appInfo.Owner.Username}#{appInfo.Owner.Discriminator}" : appInfo.Owner.Mention)}\n" +
            $"Orbital, Inc. ASN: [AS397441](https://asn.ipinfo.app/AS397441)\n" +
            $"Uptime: <t:{((DateTimeOffset)Process.GetCurrentProcess().StartTime).ToUnixTimeSeconds()}:R>\n" +
            $"Build Version: {Assembly.GetExecutingAssembly().GetName().Version}\n" +
            $"Description: {(appInfo.Description == string.Empty ? "N/A" : appInfo.Description)}\n" +
            $"Latency: {Context.Client.Latency}ms\n" +
            $"Terms of Service: {(string.IsNullOrWhiteSpace(appInfo.TermsOfService) ? "N/A" : appInfo.TermsOfService)}", deleteTimer: 180
            );
    }
}
