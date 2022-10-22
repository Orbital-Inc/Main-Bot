using System.Reflection;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using MainBot.Database;
using MainBot.Database.Models.Logs;
using MainBot.Utilities.Extensions;

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
            await _commands.RegisterCommandsGloballyAsync(true);
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
            await _commands.ExecuteCommandAsync(new ShardedInteractionContext(_client, arg), _services);
        }
        catch (Exception e)
        {
            await e.LogErrorAsync();
        }
    }

    private async Task ComponentCommandExecuted(ComponentCommandInfo arg1, IInteractionContext arg2, IResult arg3)
    {
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
                    break;
                case InteractionCommandError.BadArgs:
                    await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
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
                    await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
                    break;
                case InteractionCommandError.Unsuccessful:
                    await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
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
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
                    break;
                case InteractionCommandError.BadArgs:
                    await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
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
                    await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
                    break;
                case InteractionCommandError.Unsuccessful:
                    await arg2.ReplyWithEmbedAsync("Error Occured", arg3.ErrorReason, deleteTimer: 60);
                    break;
                default:
                    break;
            }
        }
    }
}
