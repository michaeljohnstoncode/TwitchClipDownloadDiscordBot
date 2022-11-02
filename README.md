# TwitchClipDownloadDiscordBot
Create twitch clips and download them through your own discord bot

# Installation






These 4 strings are required:
Twitch Client ID, 
Twitch Client Secret, 
Discord Bot Token, 
Discord User ID

1. Twitch Client ID and Secret

Navigate to https://dev.twitch.tv/console. Twitch will ask you to log in and authorize.

Once logged into your Twitch dev console, click register your application.

Fill out the information for registering your application.
OAuth Redirect URL **_must_** be "http://localhost:8080/redirect/"
![image](https://user-images.githubusercontent.com/92060282/199513057-35cb94ce-2a5e-4559-ab41-7a5612a8f2ae.png)

You will then be given a Twitch Client ID and Client Secret.
![image](https://user-images.githubusercontent.com/92060282/199514821-54937594-2ae0-4432-aeae-4880ff4d9f75.png)

2. Discord Bot Token

Navigate to https://discord.com/developers/applications.

Once logged in, create New Application, give it any name.

Then, head over to the "Bot" tab on the left side, and Add Bot.

Generate a new Discord Bot Token.
![image](https://user-images.githubusercontent.com/92060282/199515801-7885f136-f98c-4a5e-9a23-af2f60076471.png)

Then, match these settings to your bot.
![image](https://user-images.githubusercontent.com/92060282/199516559-18b01f35-66b5-4db1-b132-028500bc2bad.png)

3. Discord User ID

Lastly, go to your discord application https://discord.com/app

If developer mode is not already enabled, go to settings > advanced > developer mode on

Go to a server/message where you can see your discord self, right click and copy ID

![image](https://user-images.githubusercontent.com/92060282/199518939-9d08192d-9ec2-4e2c-ab45-fdd5f3aea187.png)



Arguments

![image](https://user-images.githubusercontent.com/92060282/199502320-81bd97aa-7470-48cf-bd76-597c90037664.png)

# Goal/Purpose
This program streamlines the process of getting a live twitch clip, and downloading that to file. The intention behind this program was to create content faster. It allows me to clip the streamer's reaction, and send it straight to a folder used by video editing software. And since videos are downloaded using [Youtube-DL](https://github.com/ytdl-org/youtube-dl), any [video](https://github.com/ytdl-org/youtube-dl/blob/master/docs/supportedsites.md) supported by it may also be downloaded (youtube, twitter, reddit, etc).
