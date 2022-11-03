using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipDiscordBot.Models
{

    public class RootTwitchClipInfoModel
    {
        public TwitchClipInfoModel[] Data { get; set; }
        public Pagination Pagination { get; set; }
    }

    public class Pagination
    {
    }

    public class TwitchClipInfoModel
    {
        public string Id { get; set; }
        public string Url { get; set; }

        [JsonProperty("embed_url")]
        public string EmbedUrl { get; set; }

        [JsonProperty("broadcaster_id")]
        public string BroadcasterId { get; set; }

        [JsonProperty("broadcaster_name")]
        public string BroadcasterName { get; set; }

        [JsonProperty("creator_id")]
        public string CreatorId { get; set; }

        [JsonProperty("creator_name")]
        public string CreatorName { get; set; }

        [JsonProperty("video_id")]
        public string VideoId { get; set; }

        [JsonProperty("game_id")]
        public string GameId { get; set; }

        public string Language { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("view_count")]
        public int ViewCount { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
        public int Duration { get; set; }
    }

}
