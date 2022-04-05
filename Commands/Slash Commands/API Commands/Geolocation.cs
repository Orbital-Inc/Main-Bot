using System.Runtime.InteropServices;
using Discord;
using Discord.Interactions;
using MainBot.Utilities.Extensions;
using Newtonsoft.Json;

namespace MainBot.Commands.SlashCommands.APICommands;

public class Geolocation : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly HttpClient _http;
    private readonly string _endpoint = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? $"http://localhost:1337/" : $"https://api.nebulamods.ca/";

    internal Geolocation(HttpClient http) => _http = http;

    [SlashCommand("geolocate", "Retrives the geographic location & network details of the specified host.")]
    public async Task GeoLocate(string host)
    {
        await Context.ReplyWithEmbedAsync("Geolocate Host", $"Attempting to geolocate {host}, please wait...");

        if (Uri.CheckHostName(host) is not (UriHostNameType.IPv4 or UriHostNameType.IPv6 or UriHostNameType.Dns))
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "The specified hostname/IPv4 address is not valid, please try again.", deleteTimer: 60);
            return;
        }

        //adding header for request
        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", Properties.Resources.APIToken);
        HttpResponseMessage? result = await _http.GetAsync($"{_endpoint}network-tools/geolocation/{host}");
        Models.APIModels.GeolocationModel? Information = null;
        //deserializing request response if successful
        if (result.IsSuccessStatusCode)
            Information = JsonConvert.DeserializeObject<Models.APIModels.GeolocationModel>(await result.Content.ReadAsStringAsync());

        if (Information is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "The specified hostname/IPv4 address is not valid, please try again.", deleteTimer: 60);
            return;
        }
        List<EmbedFieldBuilder> Fields = new();
        #region Security
        string ExtraInfo = string.Empty;
        if (Information.cloudProvider is not null && (bool)Information.cloudProvider)
            ExtraInfo += $"`Cloud Provider`: True\n";
        if (Information.abuser is not null && (bool)Information.abuser)
            ExtraInfo += $"`Abuser`: True\n";
        if (Information.tor is not null && (bool)Information.tor)
            ExtraInfo += $"`Tor`: True\n";
        if (Information.attacker is not null && (bool)Information.attacker)
            ExtraInfo += $"`Attacker`: True\n";
        if (Information.proxy is not null && (bool)Information.proxy)
            ExtraInfo += $"`Proxy`: True\n";
        if (Information.relay is not null && (bool)Information.relay)
            ExtraInfo += $"`Relay`: True\n";
        if (Information.annonymous is not null && (bool)Information.annonymous)
            ExtraInfo += $"`Anonymous`: True\n";
        if (Information.bogon is not null && (bool)Information.bogon)
            ExtraInfo += $"`Bogon`: True\n";
        if (Information.torExit is not null && (bool)Information.torExit)
            ExtraInfo += $"`Tor Exit`: True\n";
        if (Information.threat is not null && (bool)Information.threat)
            ExtraInfo += $"`Threat`: True\n";
        #endregion

        Fields.Add(new EmbedFieldBuilder
        {
            Name = "Network",
            Value = $"{(string.IsNullOrWhiteSpace(Information.ip) ? "" : $"`IP Address`: {Information.ip}\n")}" +
            $"{(string.IsNullOrWhiteSpace(Information.hostname) ? "" : $"`Hostname`: {Information.hostname}\n")}" +
            $"{(string.IsNullOrWhiteSpace(Information.route) ? "" : $"`Route`: {Information.route}\n")}" +
            $"{(string.IsNullOrWhiteSpace(Information.type) ? "" : $"`Type`: {Information.type}\n")}" +
            ExtraInfo
        });

        Fields.Add(new EmbedFieldBuilder
        {
            Name = "Provider",
            Value = $"{(string.IsNullOrWhiteSpace(Information.domain) ? "" : $"`Domain`: [{Information.domain}](http://{Information.domain})\n")}" +
            $"{(string.IsNullOrWhiteSpace(Information.organization) ? "" : $"`Organization`: {Information.organization}\n")}" +
            $"{(string.IsNullOrWhiteSpace(Information.isp) ? "" : $"`ISP`: {Information.isp}\n")}" +
            $"{(string.IsNullOrWhiteSpace(Information.asnName) ? "" : $"`ASN Name`: {Information.asnName}\n")}" +
            $"{(Information.asnNumber is null ? "" : $"`ASN Number`: {Information.asnNumber}\n")}"
        });

        Fields.Add(new EmbedFieldBuilder
        {
            Name = "Location",
            Value = $"{(string.IsNullOrWhiteSpace(Information.country) ? "" : $"`Country`: {Information.country}\n")}" +
            $"{(string.IsNullOrWhiteSpace(Information.region) ? "" : $"`Region`: {Information.region}\n")}" +
            $"{(string.IsNullOrWhiteSpace(Information.district) ? "" : $"`District`: {Information.district}\n")}" +
           $"{(string.IsNullOrWhiteSpace(Information.city) ? "" : $"`City`: {Information.city}\n")}"
        });

        await Context.ReplyWithEmbedAsync($"Geolocate Complete For: {host}", string.Empty, $"https://check-host.net/ip-info?host={Information.ip}", Information.flag, Fields);
    }
}
