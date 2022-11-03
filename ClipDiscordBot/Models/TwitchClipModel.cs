using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipDiscordBot.Models
{

    public class RootTwitchClipModel
    {
        public TwitchClipModel[] Data { get; set; }
    }

    public class TwitchClipModel
    {
        public string Id { get; set; }

        [JsonProperty("edit_url")]
        public string EditUrl { get; set; }
    }

}
