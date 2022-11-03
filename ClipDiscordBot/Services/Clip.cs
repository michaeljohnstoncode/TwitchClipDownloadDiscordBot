using ClipDiscordBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipDiscordBot.Services
{
    public class Clip
    {
        private readonly HttpClient _httpClient;
        private DeserializeConfigJson _configJson;

        public Clip(HttpClient httpClient, DeserializeConfigJson configJson)
        {
            _httpClient = httpClient;
            _configJson = configJson;
        }

        // call this method to get a 30 second clip url of streamer from broadcasterId
        public async Task<string> GetClipUrl(string broadcasterId)
        {
            string clipUrlJson = await CreateClipLink(broadcasterId);
            string clipUrl = DeserializeClipUrl(clipUrlJson);
            return clipUrl;
        }

        //http post request to create the video clip and return the json info for it
        public async Task<string> CreateClipLink(string broadcasterID)
        {
            HttpResponseMessage response;
            string clipUrlJson;
            string route = $"https://api.twitch.tv/helix/clips?broadcaster_id={broadcasterID}";
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), route))
            {
                var configJson = await _configJson.GetConfigJson();
                var twitchAuthToken = configJson.TwitchAuthToken;
                var twitchClientId = configJson.TwitchClientId;
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + twitchAuthToken);
                request.Headers.TryAddWithoutValidation("Client-Id", twitchClientId);
                response = await _httpClient.SendAsync(request);
                clipUrlJson = await response.Content.ReadAsStringAsync();
            }
            return clipUrlJson;
        }

        //this method deserializes the clip's json info and returs the URL
        public string DeserializeClipUrl(string clipUrlJson)
        {
            string clipUrl;

            //exception: streamer is offline, can not clip
            if (clipUrlJson.Contains("offline"))
            {
                clipUrl = "offline";
                return clipUrl;
            }

            if(clipUrlJson.Contains("User does not have permissions to Clip on this channel"))
            {
                clipUrl = "User does not have permissions to clip on this channel";
                return clipUrl;
            }

            //returns clip link
            if (clipUrlJson.Contains("url"))
            {
                var twitchClipData = JsonConvert.DeserializeObject<RootTwitchClipModel>(clipUrlJson);
                clipUrl = twitchClipData.Data.Single().EditUrl;
                clipUrl = clipUrl.Remove(clipUrl.Length - 5, 5);
                return clipUrl;
            }
            //exception: if it does not contain a url, then there is no access to clip
            else
            {
                clipUrl = "uncommon error";
                return clipUrl;
            }
        }

    }
}
