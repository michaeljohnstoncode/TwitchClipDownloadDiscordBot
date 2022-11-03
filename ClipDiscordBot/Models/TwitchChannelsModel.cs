using Newtonsoft.Json;

namespace ClipDiscordBot.Models
{

    public class RootTwitchChannelsModel
    {
        public TwitchChannelsModel[] Data { get; set; }
        public Pagination Pagination { get; set; }
    }

    public class PaginationTwitchChannelsModel
    {
        public string Cursor { get; set; }
    }


    public class TwitchChannelsModel
    {
        public TwitchChannelsModel(string displayName, string isLive)
        {
            DisplayName = displayName;
            IsLive = isLive;
        }

        [JsonProperty("broadcaster_language")]
        public string BroadcasterLanguage { get; set; }

        [JsonProperty("broadcaster_login")]
        public string BroadcasterLogin { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("game_id")]
        public string GameId { get; set; }

        [JsonProperty("game_name")]
        public string GameName { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        //originally bool in json, but easier used as string
        [JsonProperty("is_live")]
        public string IsLive { get; set; }

        [JsonProperty("tag_ids")]
        public object[] TagIds { get; set; }

        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("started_at")]
        public string StartedAt { get; set; }
    }

}
