using Discord;

namespace Main_Bot.Utilities.Extensions;

internal static class ChannelExtension
{
    internal static async Task SendEmbedAsync(this IChannel channel, string title, string description, string footer, string footerIcon, List<EmbedFieldBuilder>? embeds = null, int? deleteTimer = null)
    {
        var embed = new EmbedBuilder()
        {
            Title = title,
            Color = Miscallenous.RandomDiscordColour(),
            Author = new EmbedAuthorBuilder
            {
                Url = "https://nebulamods.ca",
                Name = "Nebula Mods Inc.",
                IconUrl = "https://nebulamods.ca/content/media/images/Home.png"
            },
            Footer = new EmbedFooterBuilder
            {
                Text = footer,
                IconUrl = footerIcon
            },
            Description = description,
        }.WithCurrentTimestamp().Build();
        if (embeds is not null)
            embed = embed.ToEmbedBuilder().WithFields(embeds).Build();
        IUserMessage msg = await (channel as ITextChannel).SendMessageAsync(embed: embed);
        try
        {
            if (deleteTimer is not null && msg is not null)
                _ = Task.Run(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds((int)deleteTimer));
                    msg.DeleteAsync();
                });
        }
        catch { }
    }
}
