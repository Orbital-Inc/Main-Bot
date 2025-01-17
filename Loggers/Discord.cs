﻿using Discord;
using Discord.WebSocket;

namespace MainBot.Loggers;

internal class DiscordLogger
{
    private readonly DiscordShardedClient _discord;
    public DiscordLogger(DiscordShardedClient discord)
    {
        _discord = discord;
        _discord.Log += Logger;
    }
    private static Task Logger(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogSeverity.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogSeverity.Info:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
        }
        Console.WriteLine($"{DateTime.Now} [{message.Severity}] {message.Source}: {message.Message} {message.Exception}");
        Console.ResetColor();
        return Task.CompletedTask;
    }
}
