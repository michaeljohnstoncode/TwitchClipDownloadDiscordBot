using ClipDiscordBot;
using ClipDiscordBot.Models;
using ClipDiscordBot.Services;
using ClipDownloadDiscordBot;
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
    private static string _configFileName = "config.json";

    // Avoid throwing out arguments
    public static async Task Main(string[] args) => await CheckReadyToRun(args);

    // Check if there are arguments and if there is a config file
    private static async Task CheckReadyToRun(string[] args)
    {
        // You should check for a filepath as argument and accept it as the json file here
        if (args.Length != 0)
        {
            Console.WriteLine("This program does not accept any arguments.");
            return;
        }
        if (!ConfigExists())
        {
            Console.WriteLine("Missing config.json file please see the readme.md for more information.");
            return;
        }
        var discord = new SetupDiscord();
        await discord.MainAsync(_configFileName);
    }

    private static bool ConfigExists() => File.Exists(_configFileName);

}