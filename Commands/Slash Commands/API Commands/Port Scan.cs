using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;
using MainBot.Utilities.Extensions;
using Newtonsoft.Json;

namespace MainBot.Commands.SlashCommands.APICommands;

public class PortScan : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly HttpClient _http;
    private readonly string _endpoint = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? $"http://localhost:1337/" : $"https://api.nebulamods.ca/";

    public PortScan(HttpClient http) => _http = http;

    [SlashCommand("port-scan", "Scan specified host to see if the specified port(s) is open using either TCP/UDP.")]
    public async Task Scan(string host, string ports = "22,53,80,443,1194")
    {
        string MainDescription = $"Attempting to port scan {host}, on the following ports: {ports}, please wait...";

        if (string.IsNullOrWhiteSpace(host))
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "The specified hostname/IPv4 address is not valid, please try again.", deleteTimer: 60);
            return;
        }

        await Context.ReplyWithEmbedAsync("Port Scan", MainDescription);

        #region Info Checks
        //max port count is 10
        string[]? portSplit = ports.Split(',');
        if (portSplit.Length > 10)
        {
            await Context.ReplyWithEmbedAsync("Port Scanner Ports Error", "The specified amount of ports is too high, please try again.");
            return;
        }
        if (Uri.CheckHostName(host) is not (UriHostNameType.IPv4 or UriHostNameType.Dns))
        {
            await Context.ReplyWithEmbedAsync("Port Scanner Invalid Host Error", "The specified hostname/IPv4 address is not valid, please try again.");
            return;
        }

        if (ports != "0")
        {
            if (!(ports.Contains(',') || ports.Contains('-')))
            {
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
        }

        #endregion

        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", Properties.Resources.APIToken);
        Models.APIModels.PortScanModel? PortScanResult = null;
        HttpResponseMessage? result = await _http.GetAsync($"{_endpoint}network/portscan/{host}/{ports}");
        if (result.IsSuccessStatusCode)
            PortScanResult = JsonConvert.DeserializeObject<Models.APIModels.PortScanModel>(await result.Content.ReadAsStringAsync());

        if (PortScanResult is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "An error occurred while attempting to port scan, please try again.", deleteTimer: 60);
            return;
        }

        string embedvalue = string.Empty;
        await PortScanResult.results.ToAsyncEnumerable().ForEachAsync(value =>
        {
            embedvalue += $"{value.protocol} to port [{value.port}](https://check-host.net/check-{value.protocol.ToLower()}?host={PortScanResult.host}%3A{value.port}) is `{value.status}`\n";
        });
        List<EmbedFieldBuilder> Fields = new();

        Fields.Add(new EmbedFieldBuilder
        {
            Name = "Port Scan Results",
            Value = embedvalue
        });

        await Context.ReplyWithEmbedAsync($"Port Scan Complete For: {PortScanResult.host}", string.Empty, string.Empty, $"https://check-host.net/ip-info?host={PortScanResult.host}", string.Empty, Fields);
    }
}
