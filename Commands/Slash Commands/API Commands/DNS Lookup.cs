using Discord;
using Discord.Interactions;

using MainBot.Utilities.Extensions;

namespace MainBot.Commands.SlashCommands.APICommands;
public class DNSLookup : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly HttpClient _http;

    internal DNSLookup(HttpClient http) => _http = http;

    [SlashCommand("dns-lookup", "Lookup a DNS record for the specified host.")]
    public async Task PingHost(string host)
    {
        await Context.ReplyWithEmbedAsync("DNS Lookup", $"Attempting to lookup {host}, please wait...");

        if (Uri.CheckHostName(host) is not (UriHostNameType.Dns))
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "The specified hostname/IPv4 address is not valid, please try again.", deleteTimer: 60, invisible: true);
            return;
        }

        //add header
        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", Properties.Resources.APIToken);
        //response
        HttpResponseMessage? result = await _http.GetAsync($"https://api.nebulamods.ca/network/dns-lookup/{host}");
        if (result.IsSuccessStatusCode)
        {
            await Context.ReplyWithEmbedAsync($"DNS Lookup Complete For: {host}", string.Empty, $"https://nebulamods.ca/geolocation?ip={host}", string.Empty, string.Empty, new List<EmbedFieldBuilder>()
            {
                new EmbedFieldBuilder()
                {
                    Name = "Domain System Name",
                    Value = await result.Content.ReadAsStringAsync()
                }
            });
        }
        else
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "An error occurred while attempting to lookup record, please try again.", deleteTimer: 60, invisible: true);
            return;
        }
    }
}
