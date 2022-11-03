using ClipDiscordBot.Models;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace ClipDiscordBot.Services
{
    public class StreamerVod
    {
        private readonly HttpClient _httpClient;
        private DeserializeConfigJson _configJson;

        public StreamerVod(HttpClient httpClient, DeserializeConfigJson configJson)
        {
            _httpClient = httpClient;
            _configJson = configJson;
        }

        //exception: cant get clip for some reason (clips disabled, follow/sub-only clips), so get the streamer's latest VOD
        public async Task<string> GetVOD(string broadcasterID)
        {
            string vodInfo = await VOD(broadcasterID);
            string vodLink = DeserializeVODLink(vodInfo);
            return vodLink;
        }

        public async Task<string> VOD(string broadcasterID)
        {
            HttpResponseMessage response;
            string responseString;
            string route = $"https://api.twitch.tv/helix/videos?user_id={broadcasterID}&first=1";
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), route))
            {
                var configJson = await _configJson.GetConfigJson();
                var twitchAuthToken = configJson.TwitchAuthToken;
                var twitchClientId = configJson.TwitchClientId;
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + twitchAuthToken);
                request.Headers.TryAddWithoutValidation("Client-Id", twitchClientId);
                response = await _httpClient.SendAsync(request);
                responseString = await response.Content.ReadAsStringAsync();
            }
            return responseString;
        }

        public string DeserializeVODLink(string vodInfo)
        {
            string vodLink;
            if (vodInfo.Contains("url"))
            {
                var twitchVodInfo = JsonConvert.DeserializeObject<RootTwitchVodModel>(vodInfo);
                vodLink = twitchVodInfo.Data.Single().Url;
                return vodLink;
            }
            else
                return vodLink = "no vod";
        }
    }
    
}
