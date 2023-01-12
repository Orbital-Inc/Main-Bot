﻿using System.Text.RegularExpressions;

using Discord;
using Discord.Interactions;

using MainBot.Utilities;
using MainBot.Utilities.Extensions;

namespace MainBot.Commands.SlashCommands.GuildCommands;

public class StealEmoteCommand : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly HttpClient _http;
    public StealEmoteCommand(HttpClient http) => _http = http;

    [SlashCommand("steal-emote", "Steal an emote from another server.")]
    public async Task StealEmoteTask(string? emote = null, string? name = null, string? imageUrl = null) => await StealEmoteAsync(emote, imageUrl, name);

    private async Task StealEmoteAsync(string? emote, string? imageUrl, string? name)
    {
        if (Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageEmojisAndStickers is false)
        {
            await Context.ReplyWithEmbedAsync("Error Occurred", "Missing permissions, please try again.", deleteTimer: 60, invisible: true);
            return;
        }
        if (string.IsNullOrWhiteSpace(emote) && string.IsNullOrWhiteSpace(imageUrl))
        {
            await Context.ReplyWithEmbedAsync("Error Occurred", "Please enter an emote or url.", deleteTimer: 60, invisible: true);
            return;
        }

        string? emoteUrl = emote;
        string? emoteName = name;
        if (string.IsNullOrWhiteSpace(emote) is false)
        {
            if (emote.StartsWith("https://") is false)
            {
                var emoteFunc = Utilities.Extensions.DiscordExtensions.ReturnEmote(emote);
                if (string.IsNullOrWhiteSpace(emoteFunc.Item1) || emoteFunc.Item2 == 0)
                {
                    await Context.ReplyWithEmbedAsync("Error Occurred", "Please enter an emote.", deleteTimer: 60, invisible: true);
                    return;
                }
                emoteUrl = $"https://cdn.discordapp.com/emojis/{emoteFunc.Item2}.{emoteFunc.Item3}?size=96";
                emoteName = emoteFunc.Item1;
            }
        }
        else if (string.IsNullOrWhiteSpace(imageUrl) is false)
        {
            emoteUrl = imageUrl;
        }
        if (string.IsNullOrWhiteSpace(emoteName) && string.IsNullOrWhiteSpace(imageUrl) is false)
        {
            await Context.ReplyWithEmbedAsync("Error Occurred", "Please enter a name.", deleteTimer: 60, invisible: true);
            return;
        }
        using var ms = new MemoryStream(await _http.GetByteArrayAsync(emoteUrl));
        if (ms.Length > 256 * 1024)
        {
            await Context.ReplyWithEmbedAsync("Error Occured", "Emoji is too big. (Sorry resizing isn't available yet)", deleteTimer: 60, invisible: true);
            return;
        }

        GuildEmote? guildemote = await Context.Guild.CreateEmoteAsync(emoteName, new Image(ms));

        await Context.ReplyWithEmbedAsync("Emote Stealer", "Successfully added emote.", "https://nebulamods.ca", guildemote.Url, string.Empty, deleteTimer: 120);
    }
}
