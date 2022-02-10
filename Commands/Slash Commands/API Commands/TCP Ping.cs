using System.Runtime.InteropServices;
using Discord;
using Discord.Interactions;
using MainBot.Utilities.Extensions;
using Newtonsoft.Json;

namespace MainBot.Commands.SlashCommands.APICommands;

public class TCPPing : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly HttpClient _http;
    private readonly string _endpoint = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? $"http://localhost/" : $"https://api.nebulamods.ca/";

    public TCPPing(HttpClient http)
    {
        _http = http;
    }

    [SlashCommand("ping-tcp", "Attempts to start & complete a TCP handshake to a specified host.")]
    public async Task TCPPingHost(string host, ushort port = 80, string server = "OVH-US")
    {
        await Context.ReplyWithEmbedAsync("TCP Ping", $"Attempting to TCP ping {host} through port {port}, please wait...");

        #region Info Checks

        if (Uri.CheckHostName(host) is not (UriHostNameType.IPv4 or UriHostNameType.IPv6 or UriHostNameType.Dns))
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "The specified hostname/IPv4 address is not valid, please try again.", deleteTimer: 60);
            return;
        }

        #endregion

        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Dank", Properties.Resources.APIToken);
        var PingResults = JsonConvert.DeserializeObject<Models.API_Models.TCPPingModel>(await _http.GetStringAsync($"{_endpoint}network-tools/tping?Host={host}&Port={port}&Server={server}"));
        if (PingResults is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "An error occurred while attempting to tcp ping, please try again.", deleteTimer: 60);
            return;
        }
        string embedvalue = string.Empty;
        await PingResults.Results.ToAsyncEnumerable().ForEachAsync(value =>
        {
            embedvalue += $"[{PingResults.Host}](https://check-host.net/check-tcp?host={PingResults.Host}%3A{PingResults.DstPort}) {(value.RecievedResponse ? $"replied back on {PingResults.DstPort} in" : "failed to reply back")}{(value.RecievedResponse ? $" `{value.ResponseTime}`\n" : "\n")}";
        });
        if (PingResults.AverageResponseTime is not null)
            embedvalue += $"Average: `{PingResults.AverageResponseTime}` Maximum: `{PingResults.MaximumResponseTime}` Minimum: `{PingResults.MinimumResponseTime}`";
        List<EmbedFieldBuilder> Fields = new();

        Fields.Add(new EmbedFieldBuilder
        {
            Name = "TCP Ping Results",
            Value = embedvalue
        });
        await Context.ReplyWithEmbedAsync($"TCP Ping Complete For: {PingResults.Host}", string.Empty, $"https://check-host.net/ip-info?host={PingResults.Host}", string.Empty, Fields);
    }
}
