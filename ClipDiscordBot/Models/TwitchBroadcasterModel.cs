using Newtonsoft.Json;

namespace ClipDiscordBot.Models
{
    public class RootTwitchBroadcasterModel
    {
        public TwitchBroadcasterModel[] Data { get; set; }
    }

    public class TwitchBroadcasterModel
    {
        public string Id { get; set; }
        public string Login { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        public string Type { get; set; }

        [JsonProperty("broadcaster_type")]
        public string BroadcasterType { get; set; }
        public string Description { get; set; }

        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [JsonProperty("offline_image_url")]
        public string OfflineImageUrl { get; set; }

        [JsonProperty("view_count")]
        public int ViewCount { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }

}
