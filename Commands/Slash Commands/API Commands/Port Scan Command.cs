using Discord;
using Discord.Interactions;
using Main_Bot.Utilities.Extensions;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Main_Bot.Commands.SlashCommands.APICommands;

public class PortScan : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly HttpClient _http;
    private readonly string Endpoint = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? $"http://localhost/" : $"https://api.nebulamods.ca/";

    public PortScan(HttpClient http)
    {
        _http = http;
    }

    [SlashCommand("port-scan", "Scan specified host to see if the specified port(s) is open using either TCP/UDP.")]
    public async Task Scan(string host, string ports = "22,53,80,443,1194", string protocol = "TCP", string server = "OVH-US")
    {
        string MainDescription = $"Attempting to port scan {host}, on the following ports: {ports}, please wait...";

        if (string.IsNullOrWhiteSpace(host))
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "The specified hostname/IPv4 address is not valid, please try again.", deleteTimer: 60);
            return;
        }

        if (!string.IsNullOrWhiteSpace(ports))
            if (Regex.IsMatch(ports, @"^[a-zA-Z]+$"))
            {
                if (ports.ToUpper() is "UDP" or "BOTH")
                {
                    protocol = ports.ToUpper();
                    ports = "21,22,53,80,443,1194,3306,3389";
                    MainDescription = $"Attempting to port scan {host}, on the following ports: {ports} using only {protocol}, please wait...";
                }
                else
                {
                    protocol = ports.ToUpper();
                    ports = "0";
                    MainDescription = $"Attempting to port scan {host} using the \"{protocol}\" template, please wait...";
                }
            }

        await Context.ReplyWithEmbedAsync("Port Scan", MainDescription);

        #region Info Checks

        if (Uri.CheckHostName(host) is not (UriHostNameType.IPv4 or UriHostNameType.Dns))
        {
            await Context.ReplyWithEmbedAsync("Port Scanner Invalid Host Error", "The specified hostname/IPv4 address is not valid, please try again.");
            return;
        }

        if (ports != "0")
        {
            if (protocol.ToUpper() is not ("UDP" or "TCP"))
                protocol = "Default";
            if (!(ports.Contains(',') || ports.Contains('-')))
                try
                {
                    Convert.ToUInt16(ports);
                }
                catch
                {
                    await Context.ReplyWithEmbedAsync("Port Scanner Invalid Port Error", "The specified port is not valid, please try again.");
                    return;
                }
        }

        #endregion

        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", Properties.Resources.API_Token);
        var PortScanResult = JsonConvert.DeserializeObject<Models.API_Models.PortScanModel>(await _http.GetStringAsync($"{Endpoint}network-tools/portscan?Host={host}&Protocol={protocol.ToUpper()}&Ports={ports}&Server={server}"));

        if (PortScanResult is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "An error occurred while attempting to port scan, please try again.", deleteTimer: 60);
            return;
        }

        string embedvalue = string.Empty;
        await PortScanResult.Results.ToAsyncEnumerable().ForEachAsync(value =>
        {
            embedvalue += $"{value.Protocol.ToUpper()} to port [{value.Port}({value.PortUsage})](https://check-host.net/check-{value.Protocol.ToLower()}?host={PortScanResult.Host}%3A{value.Port}) is `{value.Status}`\n";
        });
        List<EmbedFieldBuilder> Fields = new();

        Fields.Add(new EmbedFieldBuilder
        {
            Name = "Port Scan Results",
            Value = embedvalue
        });

        await Context.ReplyWithEmbedAsync($"Port Scan Complete For: {PortScanResult.Host}", string.Empty, $"https://check-host.net/ip-info?host={PortScanResult.Host}", string.Empty, Fields);
    }
}
