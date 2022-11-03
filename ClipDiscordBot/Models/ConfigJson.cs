using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipDiscordBot.Models
{
    public struct ConfigJson
    {
        [JsonProperty("twitch_client_id")]
        public string TwitchClientId { get; private set; }

        [JsonProperty("twitch_client_secret")]
        public string TwitchClientSecret { get; private set; }

        [JsonProperty("discord_bot_token")]
        public string DiscordBotToken { get; set; }

        [JsonProperty("discord_user_id")]
        public string DiscordUserId { get; set; }

        [JsonProperty("twitch_auth_token")]
        public string TwitchAuthToken { get; set; }

        [JsonProperty("twitch_auth_code")]
        public string TwitchAuthCode { get; set; }

        [JsonProperty("twitch_refresh_token")]
        public string TwitchRefreshToken { get; set; }

    }
}
