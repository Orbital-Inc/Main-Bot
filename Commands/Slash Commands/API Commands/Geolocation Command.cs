using Discord;
using Discord.Interactions;
using MainBot.Utilities.Extensions;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace MainBot.Commands.SlashCommands.APICommands;

public class Geolocation : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly HttpClient _http;
    private readonly string _endpoint = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? $"http://localhost/" : $"https://api.nebulamods.ca/";

    internal Geolocation(HttpClient http)
    {
        _http = http;
    }

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
        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Dank", Properties.Resources.API_Token);
        //deserializing request response if successful
        var Information = JsonConvert.DeserializeObject<Models.API_Models.GeolocationModel>(await _http.GetStringAsync($"{_endpoint}network-tools/geolocation?Host={host}"));

        if (Information is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "The specified hostname/IPv4 address is not valid, please try again.", deleteTimer: 60);
            return;
        }
        List<EmbedFieldBuilder> Fields = new();

        #region Security
        string ExtraInfo = string.Empty;
        if (Information.Cloud_Provider)
            ExtraInfo += $"`Cloud Provider`: True\n";
        if (Information.Abuser)
            ExtraInfo += $"`Abuser`: True\n";
        if (Information.Tor)
            ExtraInfo += $"`Tor`: True\n";
        if (Information.Attacker)
            ExtraInfo += $"`Attacker`: True\n";
        if (Information.Proxy)
            ExtraInfo += $"`Proxy`: True\n";
        #endregion

        Fields.Add(new EmbedFieldBuilder
        {
            Name = "Network",
            Value = $"`IP Address`: {Information.IPAddy}\n" +
            $"`Hostname`: {Information.Hostname}\n" +
            $"`Route`: {Information.Route}\n" +
            $"`Type`: {Information.Type}\n" +
            ExtraInfo
        });

        Fields.Add(new EmbedFieldBuilder
        {
            Name = "Provider",
            Value = $"`Domain`: [{Information.Domain}](http://{Information.Domain})\n" +
            $"`Organization`: {Information.Organization}\n" +
            $"`ISP`: {Information.ISP}\n" +
            $"`ASN Name`: {Information.ASN_Name}"

        });

        Fields.Add(new EmbedFieldBuilder
        {
            Name = "Location",
            Value = $"`Country`: {Information.Country}\n" +
            $"`Region`: {Information.Region}\n" +
            $"`District`: {Information.District}\n" +
            $"`City`: {Information.City}"
        });

        await Context.ReplyWithEmbedAsync($"Geolocate Complete For: {host}", string.Empty, $"https://check-host.net/ip-info?host={Information.IPAddy}", Information.Flag, Fields);
    }
}
