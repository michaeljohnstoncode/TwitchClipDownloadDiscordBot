
using ClipDownloadDiscordBot.Services;
using NYoutubeDL;
using NYoutubeDL.Options;

namespace ClipDiscordBot.Services
{
    public class DownloadClip
    {
        private YoutubeDLP _youtubeDl;
        private ClipInfo _clipInfo;
        private string lastClipUrl;
        private UpdateYTDLP _updateYoutubeDLP;

        public DownloadClip(YoutubeDLP youtubeDl, ClipInfo clipInfo, UpdateYTDLP updateYoutubeDLP)
        {
            _youtubeDl = youtubeDl;
            _clipInfo = clipInfo;
            _updateYoutubeDLP = updateYoutubeDLP;
        }

        public void SetPreviousClip(string clipUrl) => lastClipUrl = clipUrl;

        //using a library for YoutubeDL program which downloads videos from most popular websites
        //this method is used for twitch clips but can accept any other videos that YoutubeDL allows
        public async Task DownloadClipToFile(string clipUrl, Discord.Commands.SocketCommandContext context)
        {
            //get current clip download options
            _youtubeDl.Options = Options.Deserialize(File.ReadAllText("options.config"));

            // set clip link if requested command is /dl [null]. this downloads the previously requested clip from command /clip [streamerName]
            if (clipUrl == null)
            {
                clipUrl = lastClipUrl;
                if (lastClipUrl == null)
                {
                    await context.Channel.SendMessageAsync($"Can't find the previous clip. Clip URL is required.");
                    return;
                }
            }

            //set up full file path 
            string filePath = _youtubeDl.Options.FilesystemOptions.Output;

            //if program unexpectedly closes and keeps .mp4 in filePath, then it is reset 
            if (filePath.Contains(".mp4"))
                filePath = FixFilePath(filePath);
            string fileMp4 = await SetFileName(clipUrl);
            string fullFilePath = filePath + fileMp4;

            //on first time launch, or if no file path is found, then file path must be set by user
            if(filePath == String.Empty)
            {
                await context.Channel.SendMessageAsync($"A file path must be set before you download");
                return;
            }

            //this catches if there is a clip with a repeated title
            //not the best solution, it turns the repeat fileName to be file1.mp4 file12.mp4 file123.mp4 ... instead of file1.mp4 file2.mp4 file3.mp4 ...
            int increase = 0;
            while (File.Exists(fullFilePath))
            {
                increase++;
                int insertFilePathIndex = fullFilePath.Length - 4;
                fullFilePath = fullFilePath.Insert(insertFilePathIndex, increase.ToString());
            }

            //setting clip download options to config
            _youtubeDl.YoutubeDlPath = "yt-dlp.exe";
            _youtubeDl.Options.FilesystemOptions.Output = fullFilePath;
            _youtubeDl.Options.FilesystemOptions.WindowsFilenames = true;
            _youtubeDl.Options.VerbositySimulationOptions.Simulate = false;
            _youtubeDl.Options.VerbositySimulationOptions.DumpSingleJson = false;
            File.WriteAllText("options.config", _youtubeDl.Options.Serialize());

            var msg = await context.Channel.SendMessageAsync($"...");
            ulong msgId = msg.Id;
            //download the video
            try
            {
                _youtubeDl.StandardOutputEvent += (sender, output) => Console.WriteLine(output);
                _youtubeDl.StandardErrorEvent += (sender, errorOutput) => Console.WriteLine(errorOutput);
                Console.WriteLine("Starting download...");
                await context.Channel.ModifyMessageAsync(msgId, x => x.Content = "Your clip is currently being downloaded");
                
                await _youtubeDl.DownloadAsync(clipUrl);
                
                //if any errors occur in the download, then try updating yt-dlp program to the latest version (in hopes it will fix)
                List<string> errors = _youtubeDl.Info.Errors;
                if (errors.Count > 0)
                {
                    _updateYoutubeDLP.UpdateYoutubeDLP();
                    await _youtubeDl.DownloadAsync(clipUrl);
                }
                
                _youtubeDl.Options.FilesystemOptions.Output = filePath;
            }
            //task canceled exception for catching when canceling download (I don't know why this happens). creates new downloader instance
            catch (System.Threading.Tasks.TaskCanceledException ex)
            {
                Console.WriteLine($"exception: {ex}");
                _youtubeDl = new YoutubeDLP();
                await context.Channel.ModifyMessageAsync(msgId, x => x.Content = "Download has canceled :x:");
                _youtubeDl.Options.FilesystemOptions.Output = filePath;
                File.WriteAllText("options.config", _youtubeDl.Options.Serialize());
                return;
            }

            //check if download is complete
            // this isn't actually working the way its intended, and I could not figure out why. it was working before...
            // this will always send a download completion, unless download is canceled above
            if (File.Exists(fullFilePath))
            {
                await context.Channel.ModifyMessageAsync(msgId, x => x.Content = "Download has been completed :white_check_mark:");
            }

            //reset filePath to be without the .mp4 file, so it can be populated again by the next .mp4 file
            //this is backup incase the FixFilePath method called above fails
            _youtubeDl.Options.FilesystemOptions.Output = filePath;
            File.WriteAllText("options.config", _youtubeDl.Options.Serialize());
        }

        public string FixFilePath(string filePath)
        {
            int backSlashIndex = filePath.LastIndexOf('\\');
            string fixedFilePath = filePath.Substring(0, backSlashIndex);
            return fixedFilePath;
        }

        public async Task<string> SetFileName(string clipUrl)
        {
            string baseUrl = "https://clips.twitch.tv/";
            string clipId = clipUrl.Replace(baseUrl, "");
            string clipTitle;
            if (!clipId.Contains("https"))
            {
                clipTitle = await _clipInfo.GetClipTitle(clipId);
            }
            else
            {
                clipTitle = clipUrl;
            }
            //removes both slash characters so the file path doesn't create extra folders
            int backSlashIndex = clipTitle.IndexOf('/');
            int slashIndex = clipTitle.IndexOf('\\');
            while (backSlashIndex != -1 || slashIndex != -1)
            {
               clipTitle = clipTitle.Replace(("/"), String.Empty);
               clipTitle = clipTitle.Replace(("\\"), String.Empty);
               backSlashIndex = clipTitle.IndexOf('/');
               slashIndex = clipTitle.IndexOf('\\');
            }

            string fileMp4 = "\\" + clipTitle + ".mp4";
            return fileMp4;
        }

        //method for command /filepath [filePath]
        public void SetFilePath(string filePath)
        { 
            Console.WriteLine($"The filepath is now set to {filePath}");
            _youtubeDl.Options.FilesystemOptions.Output = filePath;
            File.WriteAllText("options.config", _youtubeDl.Options.Serialize());
        }

        //method for command /filepath to display current file path to user
        public string GetFilePath()
        {
            _youtubeDl.Options = Options.Deserialize(File.ReadAllText("options.config"));
            string filePath = _youtubeDl.Options.FilesystemOptions.Output;
            return filePath;
        }

        //method for command /canceldl
        public async Task CancelDownload()
        {
            Console.WriteLine("Cancelling download...");
            _youtubeDl.CancelDownload();
        }
    }
}
