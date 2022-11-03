using ClipDiscordBot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipDiscordBot.Services
{
    public class SearchChannels
    {
        private readonly HttpClient _httpClient;
        private DeserializeConfigJson _configJson;

        public SearchChannels(HttpClient httpClient, DeserializeConfigJson configJson)
        {
            _httpClient = httpClient;
            _configJson = configJson;
        }

        //this method returns 5 streamerNames (and live status) that is similar in spelling to a given streamerName. the method is called when no streamer is found while attempting to clip.
        public async Task<List<ChannelList>> RelatedChannels(string streamerName)
        {
            var responseString = await GetChannelSearch(streamerName);
            var relatedChannels = GetChannelList(responseString);
            return relatedChannels;
        }

        public async Task<string> GetChannelSearch(string streamerName)
        {
            HttpResponseMessage response;
            string responseString;
            var route = $"https://api.twitch.tv/helix/search/channels?query={streamerName}&first=5";
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

        //get a list of channels with the properties of channel name and live status
        public List<ChannelList> GetChannelList(string responseString)
        {
            var channelList = new List<ChannelList>();
            var twitchChannels = JsonConvert.DeserializeObject<RootTwitchChannelsModel>(responseString);

            foreach (TwitchChannelsModel channel in twitchChannels.Data)
            {
                string streamerName = channel.DisplayName;
                string isLive = channel.IsLive;

                //switch true/false to yes/no respectively to match question "Are they live?" from bot in discord
                if (isLive == "true")
                    isLive = "yes";
                else
                    isLive = "no";

                channelList.Add(new ChannelList()
                {
                    Streamer = streamerName,
                    IsLive = isLive
                });
            }
            return channelList;
        }
    }
}
