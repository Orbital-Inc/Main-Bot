﻿using Discord;
using Discord.Interactions;

using MainBot.Utilities.Extensions;

using Newtonsoft.Json;

namespace MainBot.Commands.SlashCommands.APICommands;

public class PortScan : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly HttpClient _http;

    public PortScan(HttpClient http) => _http = http;

    [SlashCommand("port-scan", "Scan specified host to see if the specified port(s) is open using either TCP/UDP.")]
    public async Task Scan(string host, string ports = "22,53,80,443,1194")
    {
        string MainDescription = $"Attempting to port scan {host}, on the following ports: {ports}, please wait...";

        if (string.IsNullOrWhiteSpace(host))
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "The specified hostname/IPv4 address is not valid, please try again.", deleteTimer: 60, invisible: true);
            return;
        }

        _ = await Context.ReplyWithEmbedAsync("Port Scan", MainDescription);

        #region Info Checks

        //max port count is 10
        string[]? portSplit = ports.Split(',');
        if (portSplit.Length > 10)
        {
            _ = await Context.ReplyWithEmbedAsync("Port Scanner Ports Error", "The specified amount of ports is too high, please try again.");
            return;
        }
        if (Uri.CheckHostName(host) is not (UriHostNameType.IPv4 or UriHostNameType.Dns))
        {
            _ = await Context.ReplyWithEmbedAsync("Port Scanner Invalid Host Error", "The specified hostname/IPv4 address is not valid, please try again.");
            return;
        }

        if (ports != "0")
        {
            if (!(ports.Contains(',') || ports.Contains('-')))
            {
                try
                {
                    _ = Convert.ToUInt16(ports);
                }
                catch
                {
                    _ = await Context.ReplyWithEmbedAsync("Port Scanner Invalid Port Error", "The specified port is not valid, please try again.");
                    return;
                }
            }
        }

        #endregion Info Checks

        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", Properties.Resources.APIToken);
        Models.APIModels.PortScanModel? PortScanResult = null;
        HttpResponseMessage? result = await _http.GetAsync($"http://127.0.0.1:1337/v1/network/portscan/{host}/{ports}");
        if (result.IsSuccessStatusCode)
        {
            PortScanResult = JsonConvert.DeserializeObject<Models.APIModels.PortScanModel>(await result.Content.ReadAsStringAsync());
        }

        if (PortScanResult is null)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", $"An error occurred while attempting to port scan, please try again.\nResponse Staus: {result.StatusCode}", deleteTimer: 60, invisible: true);
            return;
        }

        string embedvalue = string.Empty;
        await PortScanResult.results.ToAsyncEnumerable().ForEachAsync(value =>
        {
            embedvalue += $"{value.protocol} to port [{value.port}](https://check-host.net/check-{value.protocol.ToLower()}?host={PortScanResult.host}%3A{value.port}) is `{value.status}`\n";
        });
        List<EmbedFieldBuilder> Fields = new()
        {
            new EmbedFieldBuilder
            {
                Name = "Port Scan Results",
                Value = embedvalue
            }
        };

        _ = await Context.ReplyWithEmbedAsync($"Port Scan Complete For: {PortScanResult.host}", string.Empty, $"https://orbitalsolutions.ca/geolocation?ip={PortScanResult.host}", string.Empty, string.Empty, Fields);
    }
}