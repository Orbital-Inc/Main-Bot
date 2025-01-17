﻿using Discord;
using Discord.Interactions;

using MainBot.Utilities.Extensions;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

namespace MainBot.Commands.SlashCommands.APICommands;

public class Geolocation : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly HttpClient _http;

    private readonly IConfiguration _configuration;

    internal Geolocation(HttpClient http, IConfiguration configuration)
    {
        _configuration = configuration;
        _http = http;
    }

    [SlashCommand("geolocate", "Retrives the geographic location & network details of the specified host.")]
    public async Task GeoLocate(string host)
    {
        _ = await Context.ReplyWithEmbedAsync("Geolocate Host", $"Attempting to geolocate {host}, please wait...");

        if (Uri.CheckHostName(host) is not (UriHostNameType.IPv4 or UriHostNameType.IPv6 or UriHostNameType.Dns))
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", "The specified hostname/IPv4 address is not valid, please try again.", deleteTimer: 60, invisible: true);
            return;
        }

        //adding header for request
        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", _configuration.GetSection("General")["APIToken"]);
        HttpResponseMessage? result = await _http.GetAsync($"http://127.0.0.1:1337/v1/network/geolocation/{host}");
        Models.APIModels.GeolocationModel? Information = null;
        //deserializing request response if successful
        if (result.IsSuccessStatusCode)
        {
            Information = JsonConvert.DeserializeObject<Models.APIModels.GeolocationModel>(await result.Content.ReadAsStringAsync());
        }

        if (Information is null)
        {
            _ = await Context.ReplyWithEmbedAsync("Error Occured", $"An error occurred when attempting to geolocate the specified host, please try again.\nResponse status: {result.StatusCode}", deleteTimer: 60, invisible: true);
            return;
        }
        List<EmbedFieldBuilder> Fields = new();

        #region Security

        string ExtraInfo = string.Empty;
        if (Information.cloudProvider is not null && (bool)Information.cloudProvider)
        {
            ExtraInfo += $"`Cloud Provider`: True\n";
        }

        if (Information.abuser is not null && (bool)Information.abuser)
        {
            ExtraInfo += $"`Abuser`: True\n";
        }

        if (Information.tor is not null && (bool)Information.tor)
        {
            ExtraInfo += $"`Tor`: True\n";
        }

        if (Information.attacker is not null && (bool)Information.attacker)
        {
            ExtraInfo += $"`Attacker`: True\n";
        }

        if (Information.proxy is not null && (bool)Information.proxy)
        {
            ExtraInfo += $"`Proxy`: True\n";
        }

        if (Information.relay is not null && (bool)Information.relay)
        {
            ExtraInfo += $"`Relay`: True\n";
        }

        if (Information.annonymous is not null && (bool)Information.annonymous)
        {
            ExtraInfo += $"`Anonymous`: True\n";
        }

        if (Information.bogon is not null && (bool)Information.bogon)
        {
            ExtraInfo += $"`Bogon`: True\n";
        }

        if (Information.torExit is not null && (bool)Information.torExit)
        {
            ExtraInfo += $"`Tor Exit`: True\n";
        }

        if (Information.threat is not null && (bool)Information.threat)
        {
            ExtraInfo += $"`Threat`: True\n";
        }
        if (Information.icloudRelay is not null && (bool)Information.icloudRelay)
        {
            ExtraInfo += $"`iCloud Relay`: True\n";
        }
        if (Information.datacenter is not null && (bool)Information.datacenter)
        {
            ExtraInfo += $"`Datacenter`: True\n";
        }

        #endregion Security

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
            $"{(string.IsNullOrWhiteSpace(Information.asName) ? "" : $"`AS Name`: {Information.asName}\n")}" +
            $"{(Information.asNumber is null ? "" : $"`AS Number`: {Information.asNumber}\n")}"
        });

        Fields.Add(new EmbedFieldBuilder
        {
            Name = "Location",
            Value = $"{(string.IsNullOrWhiteSpace(Information.country) ? "" : $"`Country`: {Information.country}\n")}" +
            $"{(string.IsNullOrWhiteSpace(Information.region) ? "" : $"`Region`: {Information.region}\n")}" +
            $"{(string.IsNullOrWhiteSpace(Information.district) ? "" : $"`District`: {Information.district}\n")}" +
           $"{(string.IsNullOrWhiteSpace(Information.city) ? "" : $"`City`: {Information.city}\n")}"
        });

        _ = await Context.ReplyWithEmbedAsync($"Geolocate Complete For: {host}", string.Empty, $"https://orbitalsolutions.ca/geolocation?ip={Information.ip}", Information.flag, string.Empty, Fields);
    }
}