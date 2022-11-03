using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services)
    {
        _commands = commands;
        _client = client;
        _services = services;
    }

    public async Task InstallCommandsAsync()
    {
        _client.MessageReceived += HandleCommandAsync;
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), services: _services);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        var message = messageParam as SocketUserMessage;
        if (message == null)
            return;
        int argPos = 0;
        if (message.Author.IsBot || !message.HasCharPrefix('/', ref argPos))
            return;

        var context = new SocketCommandContext(_client, message);
        await _commands.ExecuteAsync(context, argPos, services: _services);      
    }
}