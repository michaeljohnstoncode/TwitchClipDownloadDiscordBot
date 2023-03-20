using ClipDiscordBot;
using ClipDiscordBot.Models;
using ClipDiscordBot.Services;
using ClipDownloadDiscordBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NYoutubeDL;
using System.Net.Http;
using TwitchAuthExample;
public class Program
{
    private DiscordSocketClient _client;
    private CommandService _commands;
    private CommandHandler _commandHandler;
    private IServiceProvider _services;
    private DeserializeConfigJson _configJson;

    public static Task Main(string[] args) => new Program().MainAsync();

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();

        _client.Log += Log;

        //set up services and commands
        _services = ConfigureServices();
        _commands = new CommandService();
        _commandHandler = new CommandHandler(_client, _commands, _services);
        await _commandHandler.InstallCommandsAsync();

        //set up discord bot token
        _configJson = new DeserializeConfigJson();
        ConfigJson configJson = await _configJson.GetConfigJson();
        string token = configJson.DiscordBotToken;
        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("******You must input a discord token in config.json to start the bot!******");
            Console.ReadLine();
        }

        //bot start
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        //this allows the bot to continue running
        await Task.Delay(-1);
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
            .AddSingleton<AuthTokenValidity>()
            .AddSingleton<UpdateYTDLP>();

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