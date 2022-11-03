using ClipDiscordBot;
using ClipDiscordBot.Models;
using Newtonsoft.Json;
using System.Text;

public class BroadcasterID
{
    private readonly HttpClient _httpClient;
    private DeserializeConfigJson _configJson;
    private AuthTokenValidity _authTokenValidity;

    public BroadcasterID(HttpClient httpClient, DeserializeConfigJson configJson, AuthTokenValidity authTokenValidity)
    {
        _httpClient = httpClient;
        _configJson = configJson;
        _authTokenValidity = authTokenValidity; 
    }

    //call this method to get a broadcasterID from streamerName
    public async Task<string> GetBroadcasterID(string streamerName, Discord.Commands.SocketCommandContext context)
    {
        string streamerInfo = await GetStreamerInfo(streamerName, context);
        string broadcasterID = DeserializeStreamerInfo(streamerInfo);
        return broadcasterID;
    }

    //http get request to get the streamer's info in json format from twitch
    public async Task<string> GetStreamerInfo(string streamerName, Discord.Commands.SocketCommandContext context)
    {
        HttpResponseMessage response;
        var streamerInfo = string.Empty;
        var route = $"https://api.twitch.tv/helix/users?login={streamerName}";
        using (var request = new HttpRequestMessage(new HttpMethod("GET"), route))
        {
            var configJson = await _configJson.GetConfigJson();
            string twitchAuthToken = configJson.TwitchAuthToken;
            var twitchClientId = configJson.TwitchClientId;
     
            request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + twitchAuthToken);
            request.Headers.TryAddWithoutValidation("Client-Id", twitchClientId);
            response = await _httpClient.SendAsync(request);
            streamerInfo = await response.Content.ReadAsStringAsync();
        }

        //do a check to make sure twitch auth token is still valid! it expires every ~60 days (or can be invalid for other reasons)
        if(streamerInfo.Contains("Invalid OAuth token"))
        {
            _authTokenValidity.DiscordContext(context);
            await _authTokenValidity.ValidateTokens();
            await _authTokenValidity.OAuthInvalid();
            streamerInfo = await GetStreamerInfo(streamerName, context);
        }

        if (streamerInfo.Contains("OAuth token is missing"))
        {
            _authTokenValidity.DiscordContext(context);
            bool missingTokens = await _authTokenValidity.ValidateTokens();
            if (missingTokens == true)
                return streamerInfo = null;
            streamerInfo = await GetStreamerInfo(streamerName, context);
        }

       return streamerInfo;
    }

    //this method returns the broadcasterId by deserializing the streamerInfoJson
    public string DeserializeStreamerInfo(string streamerInfo)
    {
        string broadcasterId;
        if (streamerInfo == null)
            return broadcasterId = null;
        else if (streamerInfo.Contains("id"))
        {
            var twitchBroadcasterInfo = JsonConvert.DeserializeObject<RootTwitchBroadcasterModel>(streamerInfo);
            broadcasterId = twitchBroadcasterInfo.Data.Single().Id;
            return broadcasterId;
        }
        else
            return broadcasterId = "no streamer found";
    }
}
