using ClipDiscordBot.Models;
using Discord.Commands;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TwitchAuthExample;

namespace ClipDiscordBot
{
    public class AuthTokenValidity
    {
        private readonly HttpClient _httpClient;
        private DeserializeConfigJson _configJson;
        private Discord.Commands.SocketCommandContext _discordContext;
        public AuthTokenValidity(HttpClient httpClient, DeserializeConfigJson configJson)
        {
            _httpClient = httpClient;
            _configJson = configJson;
        }

        public void DiscordContext(Discord.Commands.SocketCommandContext context) => _discordContext = context;

        // checking and validating tokens
        public async Task<bool> ValidateTokens()
        {
            var configJson = await _configJson.GetConfigJson();
            var twitchAuthToken = configJson.TwitchAuthToken;
            var twitchAuthCode = configJson.TwitchAuthCode;
            var twitchClientId = configJson.TwitchClientId;
            var twitchClientSecret = configJson.TwitchClientSecret;
            var discordBotToken = configJson.DiscordBotToken;

            if (string.IsNullOrEmpty(twitchClientId) || string.IsNullOrEmpty(twitchClientSecret) || string.IsNullOrEmpty(discordBotToken))
            {
                await _discordContext.Channel.SendMessageAsync("Make sure your Twitch Client ID, Twitch Client Secret, or Discord Bot Token are entered in the config.json file");
                //return true for bool missingTokens; because some token/s are missing
                return true;
            }

            if (string.IsNullOrEmpty(twitchAuthCode))
                await NewAuthCode(_discordContext);

            if (string.IsNullOrEmpty(twitchAuthToken))
                await NewAuthToken();

            return false;
        }

        //create new auth code to be used for creating new auth token. this requires some user action
        public async Task NewAuthCode(Discord.Commands.SocketCommandContext context)
        {
            var configJson = await _configJson.GetConfigJson();
            var twitchClientId = configJson.TwitchClientId;
            var server = new TwitchAuthExample.WebServer("http://localhost:8080/redirect/");
            await context.Channel.SendMessageAsync($"Go to this link to authorize your twitch bot: \n" +
                $"https://id.twitch.tv/oauth2/authorize" +
                $"?response_type=code" +
                $"&force_verify=true" +
                $"&client_id={twitchClientId}" +
                $"&redirect_uri=http://localhost:8080/redirect/" +
                $"&scope=clips%3Aedit+channel%3Amanage%3Apolls+channel%3Aread%3Apolls" +
                $"&state=c3ab8aa609ea11e793ae92361f002671");
            var auth = await server.Listen();
            var authCode = auth.Code;
            configJson.TwitchAuthCode = authCode;
            var json = JsonConvert.SerializeObject(configJson, Formatting.Indented);
            File.WriteAllText("config.json", json);
        }

        //creating new auth token which allows the twitch bot to use the api. this is an Authorizaztion Code Grant Flow token from Twitch
        public async Task NewAuthToken()
        {
            HttpResponseMessage response;
            string authTokenJson = String.Empty;
            var configJson = await _configJson.GetConfigJson();
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://id.twitch.tv/oauth2/token"))
            {
                var twitchClientSecret = configJson.TwitchClientSecret;
                var twitchClientId = configJson.TwitchClientId;
                string twitchAuthCode = configJson.TwitchAuthCode;

                request.Content = new StringContent($"client_id={twitchClientId}" +
                    $"&client_secret={twitchClientSecret}" +
                    $"&code={twitchAuthCode}" +
                    $"&grant_type=authorization_code" +
                    $"&redirect_uri=http://localhost:8080/redirect/");
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                response = await _httpClient.SendAsync(request);
                authTokenJson = await response.Content.ReadAsStringAsync();
            }

            var authTokenInfo = JsonConvert.DeserializeObject<TwitchAuthToken>(authTokenJson);
            string authToken = authTokenInfo.AccessToken;
            string authRefreshToken = authTokenInfo.RefreshToken;
            configJson.TwitchAuthToken = authToken;
            configJson.TwitchRefreshToken = authRefreshToken;
            var json = JsonConvert.SerializeObject(configJson, Formatting.Indented);
            File.WriteAllText("config.json", json);
            await _discordContext.Channel.SendMessageAsync("Authorization success!");
        }

        //if Auth token is invalid for some reason, or the token has expired, then create new one using the refresh token 
        public async Task OAuthInvalid()
        {
            HttpResponseMessage response;
            string authTokenJson = String.Empty;
            var configJson = await _configJson.GetConfigJson();
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://id.twitch.tv/oauth2/token"))
            {
                var twitchRefreshToken = configJson.TwitchRefreshToken;
                var twitchClientId = configJson.TwitchClientId;
                var twitchClientSecret = configJson.TwitchClientSecret;

                request.Content = new StringContent($"grant_type=refresh_token" +
                    $"&refresh_token={twitchRefreshToken}" +
                    $"&client_id={twitchClientId}" +
                    $"&client_secret={twitchClientSecret}");
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                response = await _httpClient.SendAsync(request);
                authTokenJson = await response.Content.ReadAsStringAsync();
            }
            var authTokenInfo = JsonConvert.DeserializeObject<TwitchAuthToken>(authTokenJson);
            string authToken = authTokenInfo.AccessToken;
            configJson.TwitchAuthToken = authToken;
            var json = JsonConvert.SerializeObject(configJson, Formatting.Indented);
            File.WriteAllText("config.json", json);
        }
    }
}


