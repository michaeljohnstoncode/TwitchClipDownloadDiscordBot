using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClipDiscordBot.Services
{
    public class NoStreamerFound
    {
        private SearchChannels _searchChannels;

        public NoStreamerFound(SearchChannels searchChannels)
        {
            _searchChannels = searchChannels;
        }

        //exception: no streamer has been found
        public async Task<string> SimilarStreamerNames(string streamerName, Discord.Commands.SocketCommandContext context)
        {
            await context.Channel.SendMessageAsync($"The streamer **{streamerName}** is invalid! " +
                $"Did you enter it correctly? \nHere are some channels with a similar name:");
            List<ChannelList> channels = await _searchChannels.RelatedChannels(streamerName);
            int channelIndex = 0;
            StringBuilder channelSearchMessage = new();
            ChannelList currentChannel = new();
            foreach (var channel in channels)
            {
                currentChannel = channels[channelIndex];
                string currentStreamer = currentChannel.Streamer;
                currentStreamer = UnderscoreFix(currentStreamer);
                channelSearchMessage.Append($"{channelIndex + 1}. {currentStreamer} | Are they live? {currentChannel.IsLive}\n");
                channelIndex++;
            }
            return channelSearchMessage.ToString();
        }

        //this method formats the streamer's name to be displayed in discord as a list of similar streamer names if no streamer has been found
        public string UnderscoreFix(string currentStreamer)
        {
            Regex underScorePattern = new Regex("_");
            MatchCollection matches = underScorePattern.Matches(currentStreamer);
            int firstIndexMatch = currentStreamer.IndexOf("_");
            int counter = -1;
            foreach (Match match in matches)
            {
                counter++;
                if (match.Index == firstIndexMatch)
                    currentStreamer = currentStreamer.Insert(match.Index, "\\");
                else
                    currentStreamer = currentStreamer.Insert(match.Index + counter, "\\");
            }
            return currentStreamer;
        }

    }
}
