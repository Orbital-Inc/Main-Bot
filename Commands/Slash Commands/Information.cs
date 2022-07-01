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
        await Context.ReplyWithEmbedAsync("Information",
            $"Guild Count: {Context.Client.Guilds.Count}\n" +
            $"Guild Member Count: {Context.Guild.MemberCount}\n" +
            $"Developer: {(Context.Guild is null ? $"{appInfo.Owner.Username}#{appInfo.Owner.Discriminator}" : appInfo.Owner.Mention)}\n" +
            $"Nebula Mods, Inc. ASN: [AS397441](https://asn.ipinfo.app/AS397441)" +
            $"Uptime: <t:{((DateTimeOffset)Process.GetCurrentProcess().StartTime).ToUnixTimeSeconds()}:R>\n" +
            $"Build Version: {Assembly.GetExecutingAssembly().GetName().Version}\n" +
            $"{appInfo.Description}\n" +
            $"Terms of Service: {appInfo.TermsOfService}"
            );
    }
}
