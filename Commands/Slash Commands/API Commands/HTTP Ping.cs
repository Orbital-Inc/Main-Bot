using Discord;
using Discord.Interactions;

using MainBot.Utilities.Extensions;

using Newtonsoft.Json;

namespace MainBot.Commands.SlashCommands.APICommands;
public class HTTPPing : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly HttpClient _http;
    //fix me
    internal HTTPPing(HttpClient http) => _http = http;

    [SlashCommand("ping-http", "Sends an HTTP packet to a specified host in hopes for a reponse.")]
    public async Task PingHost(string host)
    {
        _ = await Context.ReplyWithEmbedAsync("HTTP Ping", $"Attempting to HTTP ping {host}, please wait...");

        if (Uri.CheckHostName(host) is not (UriHostNameType.IPv4 or UriHostNameType.IPv6 or UriHostNameType.Dns))
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "The specified hostname/IPv4 address is not valid, please try again.", deleteTimer: 60, invisible: true);
            return;
        }

        //add header
        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", Properties.Resources.APIToken);
        //response
        HttpResponseMessage? result = await _http.GetAsync($"https://api.orbitalsolutions.ca/network/http-ping/{host}");
        Models.APIModels.ICMPPingModel? PingResults = null;
        if (result.IsSuccessStatusCode)
        {
            PingResults = JsonConvert.DeserializeObject<Models.APIModels.ICMPPingModel>(await result.Content.ReadAsStringAsync());
        }
        if (PingResults is null)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "An error occurred while attempting to ping, please try again.", deleteTimer: 60, invisible: true);
            return;
        }
        if (PingResults.results is  null)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "An error occurred while attempting to ping, please try again.", deleteTimer: 60, invisible: true);
            return;
        }
        string embedvalue = string.Empty;
        await PingResults.results.ToAsyncEnumerable().ForEachAsync(value =>
        {
            embedvalue += $"[{PingResults.host}](https://check-host.net/check-ping?host={PingResults.host}) {(value.recievedResponse ? $"replied back in `{value.responseTime}`ms" : "failed to reply back")}\n";
        });
        if (PingResults.averageResponseTime is not null)
        {
            embedvalue += $"Average: `{PingResults.averageResponseTime}`ms Maximum: `{PingResults.maximumResponseTime}`ms Minimum: `{PingResults.minimumResponseTime}`ms";
        }

        List<EmbedFieldBuilder> Fields = new()
        {
            new EmbedFieldBuilder
            {
                Name = "HTTP Ping Results",
                Value = embedvalue
            }
        };
        _ = await Context.ReplyWithEmbedAsync($"HTTP Ping Complete For: {PingResults.host}", string.Empty, $"https://orbitalsolutions.ca/geolocation?ip={PingResults.host}", string.Empty, string.Empty, Fields);
    }
}
