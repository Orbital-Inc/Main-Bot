using Discord;
using Discord.WebSocket;

using MainBot.Database;
using MainBot.Utilities.Extensions;

using Microsoft.EntityFrameworkCore;

namespace MainBot.Events;

public class MessageEventHandler
{
    private readonly DiscordShardedClient _client;
    private ulong _ownerId;
    public MessageEventHandler(DiscordShardedClient client)
    {
        _client = client;
        _client.ShardReady += ShardReady;
        _client.MessageDeleted += MessageDeleted;
        _client.MessagesBulkDeleted += MessagesDeleted;
        _client.MessageUpdated += MessageUpdated;
        _client.MessageReceived += MessageRecieved;
    }

    private async Task ShardReady(DiscordSocketClient client) => _ownerId = (await client.GetApplicationInfoAsync()).Owner.Id;

    private async Task MessageRecieved(SocketMessage arg)
    {
        _ = Task.Run(async () => await CheckMessageTextAsync(arg));
        await Task.CompletedTask;
    }

    private async ValueTask<bool> CheckMessageTextAsync(SocketMessage message)
    {
        if (message.Author.IsBot || message.Author.IsWebhook || _ownerId == message.Author.Id)
        {
            return false;
        }
        if (message.MentionedChannels.Count > 4)
        {
            await message.DeleteAsync();
            return true;
        }
        if (message.MentionedRoles.Count > 4)
        {
            await message.DeleteAsync();
            return true;
        }
        if (message.MentionedUsers.Count > 4)
        {
            await message.DeleteAsync();
            return true;
        }
        return false;
    }

    private async Task MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
    {
        try
        {
            IMessage message = arg1.Value;
            if (message is null)
            {
                return;
            }

            if (await CheckMessageTextAsync(arg2))
            {
                return;
            }

            await using var database = new DatabaseContext();
            if (_client.GetChannel(message.Channel.Id) is not SocketGuildChannel socketGuildChannel)
            {
                return;
            }

            Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == socketGuildChannel.Guild.Id);
            if (guildEntry is null)
            {
                return;
            }

            if (guildEntry.guildSettings.messageLogChannelId is null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(message.Content) || string.IsNullOrWhiteSpace(arg2.Content))
            {
                if (message.Embeds.Any())
                {
                    return;
                    //log
                }
                return;
            }
            if (message.Content == arg2.Content)
            {
                return;
            }

            SocketChannel? channel = _client.GetChannel((ulong)guildEntry.guildSettings.messageLogChannelId);
            if (channel is not null)
            {
                _ = await channel.SendEmbedAsync("Message Edited",
                    $"{message.Author.Mention}",
                    $"{message.Author.Username} | {message.Author.Id}",
                    message.Author.GetAvatarUrl(), new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "Before",
                            Value = message.Content,
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                            Name = "After",
                            Value = arg2.Content,
                            IsInline = true
                        }
                    });
            }
        }
        catch (Exception e)
        {
            await e.LogErrorAsync();
        }
    }

    private async Task MessagesDeleted(IReadOnlyCollection<Cacheable<IMessage, ulong>> arg1, Cacheable<IMessageChannel, ulong> arg2)
    {
        try
        {
            var messages = arg1.ToList();
            if (messages.Any() is false)
            {
                return;
            }

            if (_client.GetChannel(messages[0].Value.Channel.Id) is not SocketTextChannel msgChannel)
            {
                return;
            }

            await using var database = new DatabaseContext();
            Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == msgChannel.Guild.Id);
            if (guildEntry is null)
            {
                return;
            }

            if (guildEntry.guildSettings.messageLogChannelId is null)
            {
                return;
            }

            SocketChannel? channel = _client.GetChannel((ulong)guildEntry.guildSettings.messageLogChannelId);
            if (channel is not null)
            {
                _ = await channel.SendEmbedAsync("Bulk Message Delete",
                    $"{messages[0].Value.Author.Mention} deleted {arg1.Count} messages in {msgChannel.Mention}",
                    $"{messages[0].Value.Author.Username} | {messages[0].Value.Author.Id}",
                    messages[0].Value.Author.GetAvatarUrl());
            }
        }
        catch (Exception e)
        {
            await e.LogErrorAsync();
        }
    }

    private async Task MessageDeleted(Cacheable<IMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2)
    {
        try
        {
            IMessage message = arg1.Value;
            if (message is null)
            {
                return;
            }

            if (_client.GetChannel(message.Channel.Id) is not SocketGuildChannel socketGuildChannel)
            {
                return;
            }

            await using var database = new DatabaseContext();
            Database.Models.Guild? guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == socketGuildChannel.Guild.Id);
            if (guildEntry is null)
            {
                return;
            }

            if (guildEntry.guildSettings.messageLogChannelId is null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(message.Content))
            {
                if (message.Embeds.Any())
                {
                    return;
                    //log
                }
                return;
            }
            SocketChannel? channel = _client.GetChannel((ulong)guildEntry.guildSettings.messageLogChannelId);
            if (channel is not null)
            {
                _ = await channel.SendEmbedAsync("Message Deleted",
                    $"{message.Author.Mention}",
                    $"{message.Author.Username} | {message.Author.Id}",
                    message.Author.GetAvatarUrl(), new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "Message Content",
                            Value = message.Content,
                        }
                    });
            }
        }
        catch (Exception e)
        {
            await e.LogErrorAsync();
        }
    }
    //add link to edited message
}
