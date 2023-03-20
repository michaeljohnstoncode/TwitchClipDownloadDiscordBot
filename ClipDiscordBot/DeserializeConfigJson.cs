using ClipDiscordBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipDiscordBot
{
    public class DeserializeConfigJson
    {
        // this deserializes the token config ("config.json"), it is where the user inputs their twitch client secret, twitch client id, and discord bot token
        public async Task<ConfigJson> GetConfigJson()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);
            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
            return configJson;
        }
    }
}
