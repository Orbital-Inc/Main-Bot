﻿using System.Reflection;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using MainBot.Database;
using MainBot.Database.Models.Logs;
using MainBot.Utilities.Extensions;

using Microsoft.EntityFrameworkCore;

namespace MainBot.Events;

internal class InteractionEventHandler
{
    private readonly DiscordShardedClient _client;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _services;
    public InteractionEventHandler(DiscordShardedClient discord, InteractionService interactionService, IServiceProvider service)
    {
        _client = discord;
        _commands = interactionService;
        _services = service;
    }
    public async Task InitializeAsync()
    {
        // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        IEnumerable<ModuleInfo>? modules = await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        _client.ShardReady += GuildReady;
        // Process the InteractionCreated payloads to execute Interactions commands
        _client.InteractionCreated += HandleInteraction;

        // Process the command execution results 
        _commands.SlashCommandExecuted += SlashCommandExecuted;
        _commands.ContextCommandExecuted += ContextCommandExecuted;
        _commands.ComponentCommandExecuted += ComponentCommandExecuted;
    }

    private async Task GuildReady(DiscordSocketClient arg)
    {
        try
        {
            _ = await _commands.RegisterCommandsGloballyAsync(true);
        }
        catch (Exception e)
        {
            await e.LogErrorAsync();
        }
    }

    private async Task HandleInteraction(SocketInteraction arg)
    {
        try
        {
            _ = await _commands.ExecuteCommandAsync(new ShardedInteractionContext(_client, arg), _services);
        }
        catch (Exception e)
        {
            await e.LogErrorAsync();
        }
    }

    private async Task ComponentCommandExecuted(ComponentCommandInfo arg1, IInteractionContext arg2, IResult arg3)
    {
        _ = Task.Run(async () => await LogCommandAsync(arg1, arg2, arg3));
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    _ = await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
                    break;
                case InteractionCommandError.BadArgs:
                    _ = await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
                    break;
                case InteractionCommandError.Exception:
                    await using (var database = new Database.DatabaseContext())
                    {
                        var entry = new ErrorLog
                        {
                            errorTime = DateTime.UtcNow,
                            source = arg1.Name,
                            message = arg3.ErrorReason,
                        };
                        await database.AddAsync(entry);
                        await database.ApplyChangesAsync();
                    };
                    _ = await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
                    break;
                case InteractionCommandError.Unsuccessful:
                    _ = await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
                    break;
                default:
                    break;
            }
        }
    }

    private Task ContextCommandExecuted(ContextCommandInfo arg1, IInteractionContext arg2, IResult arg3)
    {
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                case InteractionCommandError.UnknownCommand:
                    // implement
                    break;
                case InteractionCommandError.BadArgs:
                    // implement
                    break;
                case InteractionCommandError.Exception:
                    // implement
                    break;
                case InteractionCommandError.Unsuccessful:
                    // implement
                    break;
                default:
                    break;
            }
        }

        return Task.CompletedTask;
    }

    private async Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3)
    {
        _ = arg2.Guild is not null
            ? Task.Run(async () => await LogCommandAsync(arg1, arg2, arg3))
            : Task.Run(async () => await LogAllCommandsAsync(arg1, arg2, arg3));
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    _ = await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
                    break;
                case InteractionCommandError.BadArgs:
                    _ = await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
                    break;
                case InteractionCommandError.Exception:
                    await using (var database = new Database.DatabaseContext())
                    {
                        var entry = new ErrorLog
                        {
                            errorTime = DateTime.UtcNow,
                            source = arg1.Name,
                            message = arg3.ErrorReason
                        };
                        await database.AddAsync(entry);
                        await database.ApplyChangesAsync();
                    };
                    _ = await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
                    break;
                case InteractionCommandError.Unsuccessful:
                    _ = await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
                    break;
                default:
                    break;
            }
        }
    }

    private async Task LogCommandAsync(ICommandInfo arg1, IInteractionContext arg2, IResult arg3)
    {
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == arg2.Guild.Id);
        if (guildEntry is null)
        {
            return;
        }
        if (guildEntry.guildSettings.commandLogChannelId is null)
        {
            return;
        }
        var commandLogChannel = await arg2.Guild.GetChannelAsync(guildEntry.guildSettings.commandLogChannelId.Value);
        _ = await commandLogChannel.SendEmbedAsync("Command Executed", $"{arg2.User.Mention} has executed {arg1.Name}\nCommand Status: {(arg3.IsSuccess ? "Success" : $"Failure: {arg3.ErrorReason}")}", $"{arg2.User.Username} | {arg2.User.Id}", arg2.User.GetAvatarUrl());
    }
    private async Task LogAllCommandsAsync(ICommandInfo arg1, IInteractionContext arg2, IResult arg3)
    {
        //create better method of doing this
        //993960228913676308
        await using var database = new DatabaseContext();
        var guildEntry = await database.Guilds.FirstOrDefaultAsync(x => x.id == 993960228913676308);
        if (guildEntry is null)
        {
            return;
        }
        if (guildEntry.guildSettings.commandLogChannelId is null)
        {
            return;
        }
        var guild = await arg2.Client.GetGuildAsync(guildEntry.id);
        var commandLogChannel = await guild.GetChannelAsync(guildEntry.guildSettings.commandLogChannelId.Value);
        _ = await commandLogChannel.SendEmbedAsync("Command Executed", $"{arg2.User.Mention} has executed {arg1.Name}\nCommand Status: {(arg3.IsSuccess ? "Success" : $"Failure: {arg3.ErrorReason}")}", $"{arg2.User.Username} | {arg2.User.Id}", arg2.User.GetAvatarUrl());
    }
}
