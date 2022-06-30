using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;
using MainBot.Utilities.Extensions;

namespace MainBot.Commands.SlashCommands.GuildCommands;

public class StealEmoteCommand : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly HttpClient _http;
    public StealEmoteCommand(HttpClient http) => _http = http;

    [SlashCommand("steal-emote", "Steal an emote from another server.")]
    public async Task StealEmoteTask(string emote, string? name = null) => await StealEmoteAsync(emote, name);

    private async Task StealEmoteAsync(string emote, string? name)
    {
        if (Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageEmojisAndStickers is false)
        {
            await Context.ReplyWithEmbedAsync("Error Occurred", "Missing permissions, please try again.", deleteTimer: 60);
            return;
        }
        if (string.IsNullOrWhiteSpace(emote))
        {
            await Context.ReplyWithEmbedAsync("Error Occurred", "Please enter an emote.", deleteTimer: 60);
            return;
        }
        string emoteUrl = emote;
        string emoteName = name;
        if (emote.StartsWith("https://") is false)
        {
            Tuple<string, ulong, string>? emoteFunc = ReturnEmote(emote);
            if (string.IsNullOrWhiteSpace(emoteFunc.Item1) || emoteFunc.Item2 == 0)
            {
                await Context.ReplyWithEmbedAsync("Error Occurred", "Please enter an emote.", deleteTimer: 60);
                return;
            }
            emoteUrl = $"https://cdn.discordapp.com/emojis/{emoteFunc.Item2}.{emoteFunc.Item3}?size=96";
            emoteName = emoteFunc.Item1;
        }
        var ms = new MemoryStream(await _http.GetByteArrayAsync(emoteUrl));
        if (ms.Length > 256 * 1024)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Emoji is too big. (Sorry resizing isn't available yet)", deleteTimer: 60);
            return;
        }

        GuildEmote? guildemote = await Context.Guild.CreateEmoteAsync(emoteName, new Image(ms));

        await Context.ReplyWithEmbedAsync("Emote Stealer", "Successfully added emote.", "https://nebulamods.ca", guildemote.Url, null, deleteTimer: 60);
    }
    private static Tuple<string, ulong, string> ReturnEmote(string str)
    {
        if (new Regex("^[:<>]*$", RegexOptions.Compiled).IsMatch(str))
        {
            return Tuple.Create(string.Empty, ulong.MinValue, string.Empty);
        }
        string[]? split = Regex.Split(str, ":");
        if (split.Length < 3)
        {
            return Tuple.Create(string.Empty, ulong.MinValue, string.Empty);
        }
        string emoteName = split.Length >= 1 ? split[1] : string.Empty;
        ulong emoteId = split.Length >= 2 ? ulong.Parse(split[2].Replace(">", "")) : ulong.MinValue;
        string fileType = split[0].Contains('a') ? "gif" : "png";
        return Tuple.Create(emoteName, emoteId, fileType);
    }
}
