﻿using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using Serilog;
using WakaBot.Core.Data;
using WakaBot.Core.Graphs;
using WakaBot.Core.Services;
using WakaBot.Core.WakaTimeAPI;
using WakaBot.Core.MessageBroker;
using Microsoft.EntityFrameworkCore;

namespace WakaBot.Core
{
    public class WakaBotService : IHostedService
    {
        private DiscordSocketClient? _client;
        private InteractionService? _interactionService;
        private IConfiguration? _configuration;
        private MessageQueue? _queue;

        private readonly DiscordSocketConfig _socketConfig = new()
        {
            GatewayIntents = GatewayIntents.All,
            AlwaysDownloadUsers = true,
        };

        public WakaBotService(MessageQueue queue)
        {
            _queue = queue;
        }

        /// <summary>
        /// Entry point for Discord bot Component of application.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {

            try
            {
                _configuration = ConfigManager.Configuration;
            }
            catch (FileNotFoundException e)
            {
                ConsoleColor originalColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = originalColor;
                Environment.Exit(1);
            }

            var services = ConfigureServices();

            _client = services.GetRequiredService<DiscordSocketClient>();
            _interactionService = services.GetRequiredService<InteractionService>();

            await _client.LoginAsync(TokenType.Bot, _configuration["token"]);
            await _client.StartAsync();

            await services.GetRequiredService<CommandHandler>().InitializeAsync();
            services.GetRequiredService<SubscriptionHandler>().Initialize();
        }

        /// <summary>
        /// Creates ServiceProvider required for dependency injection.
        /// </summary>
        /// <returns>ServiceProvider of dependency injection objects.</returns>
        private ServiceProvider ConfigureServices()
        {
            // Force Serilog to use base app directory instead of current.
            Environment.CurrentDirectory = AppContext.BaseDirectory;

            // Setup Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .CreateLogger();

            var services = new ServiceCollection();

            // setup services
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
            services.AddSingleton<CommandHandler>();
            services.AddSingleton(_socketConfig);
            services.AddSingleton<IConfiguration>(_configuration!);
            services.AddSingleton<GraphGenerator>(x => new QuickChartGenerator(_configuration!["colourURL"]));
            services.AddDbContextFactory<WakaContext>(opt =>
            {
                var provider = _configuration!["databaseProvider"];
                switch (provider?.ToLower())
                {
                    case "sqlite":
                        opt.UseSqlite(_configuration!.GetConnectionString("Sqlite"));
                        break;
                    case "mysql":
                        opt.UseMySql(_configuration.GetConnectionString("MySql"), new MySqlServerVersion(new Version(5, 7)));
                        break;
                    case "postgresql":
                        opt.UseNpgsql(_configuration!.GetConnectionString("PostgreSql"));
                        break;
                    default:
                        throw new ArgumentException("Invalid database provider specified in appsettings.json");
                }
            });
            services.AddTransient<WakaTime>();
            services.AddSingleton(_queue!);
            services.AddSingleton<SubscriptionHandler>();
            services.AddMemoryCache();
            services.AddLogging(config => config.AddSerilog());

            // setup http clients
            services.AddHttpClient<WakaTime>(c => c.BaseAddress = new Uri("https://wakatime.com/api/v1/"))
                .AddHttpMessageHandler<WakaTimeCacheHandler>();

            services.AddTransient<WakaTimeCacheHandler>();

            return services.BuildServiceProvider();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
