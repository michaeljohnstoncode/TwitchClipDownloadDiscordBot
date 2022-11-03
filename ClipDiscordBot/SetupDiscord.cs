using ClipDiscordBot;
using ClipDiscordBot.Models;
using ClipDiscordBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NYoutubeDL;
using System.Net.Http;
using TwitchAuthExample;

namespace ClipDownloadDiscordBot
{
    internal class SetupDiscord
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private CommandHandler _commandHandler;
        private IServiceProvider _services;
        private DeserializeConfigJson _configJson;

        public async Task MainAsync(string configFileName)
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;

            //set up services and commands
            _services = ConfigureServices();
            _commands = new CommandService();
            _commandHandler = new CommandHandler(_client, _commands, _services);
            await _commandHandler.InstallCommandsAsync();

            //set up discord bot token
            var token = await GetToken(configFileName);

            //bot start
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            //this allows the bot to continue running
            await Task.Delay(-1);
        }

        private async Task<string> GetToken(string configFileName)
        {
            //attempt safe deserialization
            try
            {
                _configJson = new DeserializeConfigJson(configFileName);
                ConfigJson configJson = await _configJson.GetConfigJson();
                string token = configJson.DiscordBotToken;
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("******You must input a discord token in config.json to start the bot!******");
                    throw new ArgumentNullException("Missing bot token in config file.");
                }
                return token;
            }
            catch (JsonSerializationException jEx)
            {
                Console.WriteLine("JSON format not recognized.");
                var message = string.Format("Outer Exception: {0}{1}{2}", jEx.Message, Environment.NewLine, jEx.InnerException?.Message);
                Console.WriteLine(message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unknown serialization exception.");
                throw;
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton<BroadcasterID>()
                .AddSingleton<Clip>()
                .AddSingleton<SearchChannels>()
                .AddSingleton<StreamerVod>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<DownloadClip>()
                .AddSingleton<YoutubeDLP>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<ClipInfo>()
                .AddSingleton<DeserializeConfigJson>()
                .AddSingleton<NoStreamerFound>()
                .AddSingleton<AuthTokenValidity>();

            services.AddHttpClient<BroadcasterID>();
            services.AddHttpClient<Clip>();
            services.AddHttpClient<SearchChannels>();
            services.AddHttpClient<StreamerVod>();

            return services.BuildServiceProvider();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
