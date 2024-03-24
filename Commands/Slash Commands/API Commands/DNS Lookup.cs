using Discord;
using Discord.Interactions;

using MainBot.Utilities.Extensions;

using Newtonsoft.Json;

namespace MainBot.Commands.SlashCommands.APICommands;

public class DNSLookup : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly HttpClient _http;

    internal DNSLookup(HttpClient http) => _http = http;

    [SlashCommand("dns-lookup", "Lookup a DNS record for the specified host.")]
    public async Task PingHost(string host)
    {
        _ = await Context.ReplyWithEmbedAsync("DNS Lookup", $"Attempting to lookup {host}, please wait...");

        if (Uri.CheckHostName(host) is not UriHostNameType.Dns)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "The specified hostname/IPv4 address is not valid, please try again.", deleteTimer: 60, invisible: true);
            return;
        }

        //add header
        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", Properties.Resources.APIToken);
        //response
        HttpResponseMessage? result = await _http.GetAsync($"http://127.0.0.1:1337/v1/network/dnslookup/{host}");
        if (result.IsSuccessStatusCode)
        {
            var Information = JsonConvert.DeserializeObject<dynamic>(await result.Content.ReadAsStringAsync());
            _ = await Context.ReplyWithEmbedAsync($"DNS Lookup Complete For: {host}", string.Empty, $"https://orbitalsolutions.ca/geolocation?ip={host}", string.Empty, string.Empty, new List<EmbedFieldBuilder>()
            {
                new EmbedFieldBuilder()
                {
                    Name = "Domain System Name",
                    Value = Information.details
                }
            });
        }
        else
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "An error occurred while attempting to lookup record, please try again.", deleteTimer: 60, invisible: true);
            return;
        }
    }
}