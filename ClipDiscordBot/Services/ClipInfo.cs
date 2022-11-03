using ClipDiscordBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipDiscordBot.Services
{
    public class ClipInfo
    {
        private readonly HttpClient _httpClient;
        private DeserializeConfigJson _configJson;

        public ClipInfo(HttpClient httpClient, DeserializeConfigJson configJson)
        {
            _httpClient = httpClient;
            _configJson = configJson;
        }

        //this method returns clip title using the clipId, in order to give the downloaded file a name
        public async Task<string> GetClipTitle(string clipId)
        {
            string clipInfoJson = await CreateClipTitle(clipId);
            string clipTitle = DeserializeClipTitle(clipInfoJson, clipId);
            return clipTitle;
        }

        public async Task<string> CreateClipTitle(string clipId)
        {
            HttpResponseMessage response;
            string clipInfoJson;
            string route = $"https://api.twitch.tv/helix/clips?id={clipId}";
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), route))
            {
                var configJson = await _configJson.GetConfigJson();
                var twitchAuthToken = configJson.TwitchAuthToken;
                var twitchClientId = configJson.TwitchClientId;
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + twitchAuthToken);
                request.Headers.TryAddWithoutValidation("Client-Id", twitchClientId);
                response = await _httpClient.SendAsync(request);
                clipInfoJson = await response.Content.ReadAsStringAsync();
            }

            return clipInfoJson;
        }

        public string DeserializeClipTitle(string clipInfoJson, string clipId)
        {
            // if clip title GET fails, title will equal URL
            if (!clipInfoJson.Contains("broadcaster_id"))
                return clipId;

            try
            {
                var twitchClipinfo = JsonConvert.DeserializeObject<RootTwitchClipInfoModel>(clipInfoJson);
                string clipTitle = twitchClipinfo.Data.Single().Title;
                return clipTitle;
            }
            catch(Newtonsoft.Json.JsonSerializationException ex)
            { 
                Console.WriteLine(ex);
            }
            // if clipTitle can't be returned for some reason, return the clipId
            return clipId;
        }
    }
}
