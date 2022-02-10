using System.Runtime.InteropServices;
using Discord;
using Discord.Interactions;
using MainBot.Utilities.Extensions;
using Newtonsoft.Json;

namespace MainBot.Commands.SlashCommands.APICommands;

public class ICMPPing : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly HttpClient _http;
    private readonly string _endpoint = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? $"http://localhost/" : $"https://api.nebulamods.ca/";

    internal ICMPPing(HttpClient http)
    {
        _http = http;
    }

    [SlashCommand("ping-icmp", "Sends an ICMP packet to a specified host in hopes for a reponse.")]
    public async Task PingHost(string host, string server = "OVH-US")
    {
        await Context.ReplyWithEmbedAsync("ICMP Ping", $"Attempting to ICMP ping {host}, please wait...");

        if (Uri.CheckHostName(host) is not (UriHostNameType.IPv4 or UriHostNameType.IPv6 or UriHostNameType.Dns))
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "The specified hostname/IPv4 address is not valid, please try again.", deleteTimer: 60);
            return;
        }

        //add header
        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Dank", Properties.Resources.APIToken);
        //response
        var PingResults = JsonConvert.DeserializeObject<Models.API_Models.ICMPPingModel>(await _http.GetStringAsync($"{_endpoint}network-tools/ping?Host={host}&Server={server}"));

        if (PingResults is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "An error occurred while attempting to ping, please try again.", deleteTimer: 60);
            return;
        }
        string embedvalue = string.Empty;
        await PingResults.Results.ToAsyncEnumerable().ForEachAsync(value =>
        {
            embedvalue += $"[{PingResults.Host}](https://check-host.net/check-ping?host={PingResults.Host}) {(value.RecievedResponse ? $"replied back in" : "failed to reply back")}{(value.RecievedResponse ? $" `{value.ResponseTime}`\n" : "\n")}";
        });
        if (PingResults.AverageResponseTime is not null)
            embedvalue += $"Average: `{PingResults.AverageResponseTime}` Maximum: `{PingResults.MaximumResponseTime}` Minimum: `{PingResults.MinimumResponseTime}`";
        List<EmbedFieldBuilder> Fields = new();

        Fields.Add(new EmbedFieldBuilder
        {
            Name = "ICMP Ping Results",
            Value = embedvalue
        });
        await Context.ReplyWithEmbedAsync($"ICMP Ping Complete For: {PingResults.Host}", string.Empty, $"https://check-host.net/ip-info?host={PingResults.Host}", string.Empty, Fields);
    }
}
