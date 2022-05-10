﻿using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using WakaBot.Services;
using Discord.Commands;

namespace WakaBot;

public class WakaBot
{
    public static Task Main(string[] args) => new WakaBot().MainAsync();

    private DiscordSocketClient? _client;
    private InteractionService? _interactionService;

    private readonly DiscordSocketConfig _socketConfig = new()
    {
        GatewayIntents = GatewayIntents.All,
        AlwaysDownloadUsers = true,
    };

    public async Task MainAsync()
    {
        using var services = ConfigureServices();
        _client = services.GetRequiredService<DiscordSocketClient>();
        _interactionService = services.GetRequiredService<InteractionService>();

        _client.Log += Log;
        _client.Ready += ClientReady;

        var token = File.ReadAllText("token.txt");

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await services.GetRequiredService<CommandHandler>().InitializeAsync();

        await Task.Delay(-1);
    }

    private Task Log(LogMessage message)
    {
        if (message.Exception is CommandException cmdException)
        {
            Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                + $" failed to execute in {cmdException.Context.Channel}.");
            Console.WriteLine(cmdException);
        }
        else
            Console.WriteLine($"[General/{message.Severity}] {message}");

        return Task.CompletedTask;
    }

    private async Task ClientReady()
    {
        // Huawei Comp
        //await _interactionService.RegisterCommandsToGuildAsync(753255439403319326);

        // homework > /dev/urandom
        await _interactionService.RegisterCommandsToGuildAsync(771735942981615616);

    }

    private ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<CommandHandler>()
            .AddSingleton(_socketConfig)
            .BuildServiceProvider();
    }
}
