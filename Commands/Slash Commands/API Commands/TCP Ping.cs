﻿using Discord;
using Discord.Interactions;

using MainBot.Utilities.Extensions;

using Newtonsoft.Json;

namespace MainBot.Commands.SlashCommands.APICommands;

public class TCPPing : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly HttpClient _http;

    public TCPPing(HttpClient http) => _http = http;

    [SlashCommand("ping-tcp", "Attempts to start & complete a TCP handshake to a specified host.")]
    public async Task TCPPingHost(string host, ushort port = 80)
    {
        _ = await Context.ReplyWithEmbedAsync("TCP Ping", $"Attempting to TCP ping {host} through port {port}, please wait...");

        #region Info Checks

        if (Uri.CheckHostName(host) is not (UriHostNameType.IPv4 or UriHostNameType.IPv6 or UriHostNameType.Dns))
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "The specified hostname/IPv4 address is not valid, please try again.", deleteTimer: 60, invisible: true);
            return;
        }

        #endregion

        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", Properties.Resources.APIToken);
        Models.APIModels.TCPPingModel? PingResults = null;
        HttpResponseMessage? result = await _http.GetAsync($"https://api.orbitalsolutions.ca/network/tcp-ping/{host}/{port}");
        if (result.IsSuccessStatusCode)
        {
            PingResults = JsonConvert.DeserializeObject<Models.APIModels.TCPPingModel>(await result.Content.ReadAsStringAsync());
        }

        if (PingResults is null)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "An error occurred while attempting to tcp ping, please try again.", deleteTimer: 60, invisible: true);
            return;
        }
        string embedvalue = string.Empty;
        await PingResults.results.ToAsyncEnumerable().ForEachAsync(value =>
        {
            embedvalue += $"[{PingResults.host}](https://check-host.net/check-tcp?host={PingResults.host}%3A{PingResults.dstPort}) {(value.recievedResponse ? $"replied back on {PingResults.dstPort} in" : $"failed to reply back on {PingResults.dstPort}")}{(value.recievedResponse ? $" `{value.responseTime}`ms\n" : "\n")}";
        });
        if (PingResults.averageResponseTime is not null)
        {
            embedvalue += $"Average: `{PingResults.averageResponseTime}`ms Maximum: `{PingResults.maximumResponseTime}`ms Minimum: `{PingResults.minimumResponseTime}`ms";
        }

        List<EmbedFieldBuilder> Fields = new()
        {
            new EmbedFieldBuilder
            {
                Name = "TCP Ping Results",
                Value = embedvalue
            }
        };
        _ = await Context.ReplyWithEmbedAsync($"TCP Ping Complete For: {PingResults.host}", string.Empty, $"https://orbitalsolutions.ca/geolocation?ip={PingResults.host}", string.Empty, string.Empty, Fields);
    }
}
