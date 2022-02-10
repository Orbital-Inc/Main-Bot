using System.Net;
using Discord.Interactions;
using MainBot.Database;
using MainBot.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MainBot.Commands.SlashCommands;

[Utilities.Attributes.RequireDeveloper]
public class HiddenCommands : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("guilds", "Displays a list of guilds")]
    public async Task ListDiscordServersCommand()
    {
        await Context.Interaction.DeferAsync();
        //await AddUserToGuild();
        await using var database = new DatabaseContext();
        var serverDetails = string.Empty;
        await Context.Client.Guilds.ToAsyncEnumerable().ForEachAwaitAsync(async guild =>
        {
            serverDetails += await database.Guilds.FirstOrDefaultAsync(x => x.id == guild.Id) is not null ?
            $"{guild.Name} | {guild.Id} | {guild.MemberCount} ~ {guild.Owner.Username}#{guild.Owner.Discriminator} | BACKED UP\n"
            : $"{guild.Name} | {guild.Id} | {guild.MemberCount} ~ {guild.Owner.Username}#{guild.Owner.Discriminator}\n";
        });
        await Context.ReplyWithEmbedAsync("Server List", serverDetails, deleteTimer: 120);
    }
    public class Member
    {
        public int id { get; set; }
        public ulong userid { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public ulong? server { get; set; }
    }
    public class Server
    {
        public int id { get; set; }
        public string? owner { get; set; }
        public string? name { get; set; }
        public ulong? guildid { get; set; }
        public ulong? roleid { get; set; }
    }
    internal static async Task<HttpStatusCode> AddUserToGuild()
    {
        try
        {
            var user = new Member
            {
                id = 1,
                access_token = "tlqntXTWoCy8N4OIstmVExvPBw7QpQ",
                server = 933012831534735451,
                userid = 922257546264326144,
                refresh_token = "fZjS9uTPNgJCTtJQ3GGeUjF0Ncz4wG"
            };
            var server = new Server
            {
                id = 1,
                guildid = 933012831534735451,
                //roleid = 933033710444507158
            };
            using var http = new HttpClient();
            string? data = null;
            data = server.roleid switch
            {
                null => JsonConvert.SerializeObject(new { user.access_token }),
                _ => JsonConvert.SerializeObject(new
                {
                    user.access_token,
                    roles = new ulong[]
                    {
                        (ulong)server.roleid
                    },
                }),
            };
            var content = new StringContent(data);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bot", $"NzkxMTA2MDE4MTc1NjE0OTg4.X-KU5A.FMGrIUyPj89qN8FgdKxlymgA2aI");
            http.DefaultRequestHeaders.TryAddWithoutValidation("X-RateLimit-Precision", "millisecond");
            http.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "RestoreCord (public release, 1.0.0.0)");
            Console.WriteLine(await content.ReadAsStringAsync());
            foreach (var header in content.Headers)
                Console.WriteLine($"{header.Key}|{header.Value.First()}");
            var response = await http.PutAsync($"https://discord.com/api/guilds/{server.guildid}/members/{user.userid}", content);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            foreach (var header in response.Headers)
                Console.WriteLine($"{header.Key}|{header.Value.First()}");
            Console.WriteLine(response.StatusCode);
            return response.StatusCode switch
            {
                HttpStatusCode.Created or HttpStatusCode.NoContent => HttpStatusCode.OK,
                _ => response.StatusCode,//Console.WriteLine($"add user exception {response.StatusCode}\n{webex}\n{response}");
            };
        }
        catch (WebException webex)
        {
            if (webex.Response is not null)
            {
                var response = (HttpWebResponse)webex.Response;
                switch (response.StatusCode)
                {
                    case HttpStatusCode.TooManyRequests:
                        return response.StatusCode;
                    case HttpStatusCode.NoContent:
                        return HttpStatusCode.OK;
                    case HttpStatusCode.Created:
                        return HttpStatusCode.OK;
                    default:
                        Console.WriteLine($"add user exception {response.StatusCode}\n{webex}\n{response}");
                        return response.StatusCode;
                }
            }
            else
            {
                return HttpStatusCode.BadRequest;
            }
        }
    }
    [SlashCommand("rainbow-refresh", "Randomly sets rainbow role colour")]
    public async Task SwapRainbowRoleColour()
    {
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == Context.Guild.Id);
        if (guildEntry is null)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "This requires the guild to be backed up.", deleteTimer: 60, invisible: true);
            return;
        }
        if (guildEntry.guildSettings.rainbowRoleId is not null)
        {
            var role = Context.Guild.GetRole((ulong)guildEntry.guildSettings.rainbowRoleId);
            await role.ModifyAsync(x =>
            {
                x.Color = Utilities.Miscallenous.RandomDiscordColour();
            });
            await Context.Interaction.DeferAsync(true);
            return;
        }
        await Context.ReplyWithEmbedAsync("Error Occured", "Rainbow role is not set");
    }
}
