using Newtonsoft.Json;

namespace ClipDiscordBot.Models
{

    public class RootTwitchVodModel
    {
        public TwitchVodModel[] Data { get; set; }
        public Pagination Pagination { get; set; }
    }
    public class Paginationn
    {
        public string Cursor { get; set; }
    }

    public class TwitchVodModel
    {
        public string Id { get; set; }

        [JsonProperty("stream_id")]
        public string StreamId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("user_login")]
        public string UserLogin { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("published_at")]
        public DateTime PublishedAt { get; set; }
        public string Url { get; set; }

        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
        public string Viewable { get; set; }

        [JsonProperty("view_count")]
        public int ViewCount { get; set; }
        public string Language { get; set; }
        public string Type { get; set; }
        public string Duration { get; set; }

        [JsonProperty("muted_segments")]
        public object MutedSegments { get; set; }
    }

}
