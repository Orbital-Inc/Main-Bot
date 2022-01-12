using Discord;
using Discord.Interactions;
using Main_Bot.Utilities.Extensions;
using System.Text.RegularExpressions;

namespace Main_Bot.Commands.SlashCommands.GuildCommands;

public class StealEmoteCommand : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly HttpClient _http;
    public StealEmoteCommand(HttpClient http)
    {
        _http = http;
    }

    [SlashCommand("steal-emote", "Steal an emote from another server.")]
    public async Task StealEmoteTask(string emote) => await StealEmoteAsync(emote);

    private async Task StealEmoteAsync(string emote)
    {
        if (string.IsNullOrWhiteSpace(emote))
        {
            await Context.ReplyWithEmbedAsync("Error Occurred", "Please enter an emote.", deleteTimer: 60);
            return;
        }
        var emoteFunc = ReturnEmote(emote);
        if (string.IsNullOrWhiteSpace(emoteFunc.Item1) || emoteFunc.Item2 == 0)
        {
            await Context.ReplyWithEmbedAsync("Error Occurred", "Please enter an emote.", deleteTimer: 60);
            return;
        }
        var ms = new MemoryStream(await _http.GetByteArrayAsync($"https://cdn.discordapp.com/emojis/{emoteFunc.Item2}.{emoteFunc.Item3}?size=96"));
        if (ms.Length > 256 * 1024)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Emoji is too big. (Sorry resizing isn't available yet)", deleteTimer: 60);
            return;
        }
        var guildemote = await Context.Guild.CreateEmoteAsync(emoteFunc.Item1, new Image(ms));
        await Context.ReplyWithEmbedAsync("Emote Stealer", "Successfully added emote.", "https://nebulamods.ca", guildemote.Url, null, deleteTimer: 60);
    }
    private static Tuple<string, ulong, string> ReturnEmote(string str)
    {
        if (new Regex("^[:<>]*$", RegexOptions.Compiled).IsMatch(str))
        {
            return Tuple.Create(string.Empty, ulong.MinValue, string.Empty);
        }
        var split = Regex.Split(str, ":");
        if (split.Length < 3)
        {
            return Tuple.Create(string.Empty, ulong.MinValue, string.Empty);
        }
        string emoteName = split.Length >= 1 ? split[1] : null;
        ulong emoteId = split.Length >= 2 ? ulong.Parse(split[2].Replace(">", "")) : ulong.MinValue;
        string fileType = split[0].Contains('a') ? "gif" : "png";
        return Tuple.Create(emoteName, emoteId, fileType);
    }
}
