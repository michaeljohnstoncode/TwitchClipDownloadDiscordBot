using ClipDiscordBot.Services;
using Discord;
using Discord.Commands;

namespace ClipDiscordBot.Module
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private BroadcasterID _broadcasterID;
        private Clip _getClip;
        private StreamerVod _streamerVod;
        private DownloadClip _downloadClip;
        private NoStreamerFound _noStreamerFound;
        private AuthTokenValidity _authTokenValidity;
        private DeserializeConfigJson _configJson;

        public Commands(BroadcasterID broadcasterID, Clip getClip, StreamerVod streamerVod, DownloadClip downloadClip, NoStreamerFound noStreamerFound, AuthTokenValidity authTokenValidity, DeserializeConfigJson configJson)
        {
            _broadcasterID = broadcasterID;
            _getClip = getClip;
            _streamerVod = streamerVod;
            _downloadClip = downloadClip;
            _noStreamerFound = noStreamerFound;
            _authTokenValidity = authTokenValidity;
            _configJson = configJson;
        }

        [Command("help")]
        [Summary("Returns a list of all commands and their descriptions")]
        public async Task HelpCommandsAsync()
        {
            //only accept commands from ONE specific user, that who is hosting this bot
            // todo: this user.id check is repeated for every command... is there a better way?
            var configJson = await _configJson.GetConfigJson();
            string discordUserId = configJson.DiscordUserId;
            ulong id = Convert.ToUInt64(discordUserId);
            if (Context.User.Id != id)
                return;

            var builder = new EmbedBuilder()
                .WithColor(new Color(0x5622C7))
                .WithAuthor(author => {
                    author.WithName("Help Command List")
                          .WithIconUrl("http://imperfectspirituality.com/wp-content/uploads/2011/01/Smiley-face.jpg");
                })
                .AddField("/auth", "Authorizes and/or checks validity of tokens. Enter this command on first time startup.")
                .AddField("/clip [streamerName]", "Returns a 30 second twitch clip from a given streamer name")
                .AddField("/dl [URL]", "Downloads the given video URL")
                .AddField("/dl", "Downloads the previously clipped video. No URL input")
                .AddField("/canceldl", "Cancels the current download")
                .AddField("/filepath [filePath]", "Sets the file path from the given input")
                .AddField("/filepath", "Displays the current file path");

            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);
        }

        [Command("clip")]
        [Summary("Returns a 30 second twitch clip from given streamer name")]
        public async Task SendClipAsync(string streamerName)
        {
            //only accept commands from ONE specific user, that who is hosting this bot
            var configJson = await _configJson.GetConfigJson();
            string discordUserId = configJson.DiscordUserId;
            ulong id = Convert.ToUInt64(discordUserId);
            if (Context.User.Id != id)
                return;

            var context = Context;

            //set up the broadcasterID
            string broadcasterID = await _broadcasterID.GetBroadcasterID(streamerName, context);

            //exception: check config.json! twitchClientId, twitchClientSecret, and/or discordBotToken may not be set in config.json
            if(broadcasterID == null)
                return;

            //exception: no streamer has been found
            if (broadcasterID == "no streamer found")
            {
               string noStreamerFound = await _noStreamerFound.SimilarStreamerNames(streamerName, context);
               await Context.Channel.SendMessageAsync(noStreamerFound);
               return;
            }

            //streamer is found, try clipping their stream
            string clipUrl = await _getClip.GetClipUrl(broadcasterID);

            //exception: no permission to clip on this channel
            if (clipUrl == "User does not have permissions to clip on this channel")
            {
                await Context.Channel.SendMessageAsync("You do not have permission to clip on this channel");
                return;
            }
          
            //exception: cant get clip for some reason (clips disabled, follow/sub-only clips), so print the streamer's latest VOD
            if (clipUrl == "uncommon error")
            {
                string vodLink = await _streamerVod.GetVOD(broadcasterID);
                if (vodLink == "no vod")
                    await Context.Channel.SendMessageAsync("Can't get the clip, and some special access may be required to view their VOD");
                else
                    await Context.Channel.SendMessageAsync($"Unable to clip {streamerName}, here is their latest vod: {vodLink}");
            }

            //exception: streamer is offline, else print the clipUrl
            if (clipUrl == "offline")
            {
                await Context.Channel.SendMessageAsync($"I am unable to clip the streamer **{streamerName}**, they are offline.");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Here is your clip for {streamerName}: {clipUrl}");
                await Context.Channel.SendMessageAsync($"If you want to download this clip, type /dl");
                //set previousClip in order to populate the command "/dl"
                _downloadClip.SetPreviousClip(clipUrl);
            }
        }

        [Command("dl")]
        [Summary("Downloads the given clip to local files.")]
        public async Task DownloadClipAsync(string clipUrl)
        {
            //only accept commands from ONE specific user, that who is hosting this bot
            var configJson = await _configJson.GetConfigJson();
            string discordUserId = configJson.DiscordUserId;
            ulong id = Convert.ToUInt64(discordUserId);
            if (Context.User.Id != id)
                return;

            var context = Context;
            _downloadClip.DownloadClipToFile(clipUrl, context);
        }

        [Command("dl")]
        [Summary("Downloads the previously clipped video. No clipUrl input")]
        public async Task DownloadClipAsync()
        {
            //only accept commands from ONE specific user, that who is hosting this bot
            var configJson = await _configJson.GetConfigJson();
            string discordUserId = configJson.DiscordUserId;
            ulong id = Convert.ToUInt64(discordUserId);
            if (Context.User.Id != id)
                return;

            //wait 4 seconds before starting download because when a clip is created, it is not ready to be downloaded immediately
            Console.WriteLine("Download will start in 4 seconds...");
            var msg = await Context.Channel.SendMessageAsync($"Attempting download...");
            Thread.Sleep(4000);
            await Context.Channel.DeleteMessageAsync(msg.Id);

            string clipUrl = null;
            var context = Context;
            _downloadClip.DownloadClipToFile(clipUrl, context);
        }

        [Command("filepath")]
        [Summary("Sets the file path for the clip to be downloaded to")]
        public async Task FilePathAsync(string filePath)
        {
            //only accept commands from ONE specific user, that who is hosting this bot
            var configJson = await _configJson.GetConfigJson();
            string discordUserId = configJson.DiscordUserId;
            ulong id = Convert.ToUInt64(discordUserId);
            if (Context.User.Id != id)
                return;

            _downloadClip.SetFilePath(filePath);
            await Context.Channel.SendMessageAsync($"Your clips will now be sent to {filePath}");
        }

        [Command("filepath")]
        [Summary("Displays current file path")]
        public async Task GetFilePathAsync()
        {
            //only accept commands from ONE specific user, that who is hosting this bot
            var configJson = await _configJson.GetConfigJson();
            string discordUserId = configJson.DiscordUserId;
            ulong id = Convert.ToUInt64(discordUserId);
            if (Context.User.Id != id)
                return;

            string filePath = _downloadClip.GetFilePath();
            await Context.Channel.SendMessageAsync($"Your current file path is: {filePath}");
        }


        [Command("canceldl")]
        [Summary("Cancels the current download")]
        public async Task CancelDownloadAsync()
        {
            //only accept commands from ONE specific user, that who is hosting this bot
            var configJson = await _configJson.GetConfigJson();
            string discordUserId = configJson.DiscordUserId;
            ulong id = Convert.ToUInt64(discordUserId);
            if (Context.User.Id != id)
                return;

            await _downloadClip.CancelDownload();
        }

        [Command("auth")]
        [Summary("Authorizes and/or checks validity of tokens")]
        public async Task Auth()
        {
            //only accept commands from ONE specific user, that who is hosting this bot
            var configJson = await _configJson.GetConfigJson();
            string discordUserId = configJson.DiscordUserId;
            ulong id = Convert.ToUInt64(discordUserId);
            if (Context.User.Id != id)
                return;

            var context = Context;
            _authTokenValidity.DiscordContext(context);
            await _authTokenValidity.ValidateTokens();

        }
    }
}
