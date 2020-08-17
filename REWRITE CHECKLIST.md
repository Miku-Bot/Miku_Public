# Miku Bot Rewrite Checklist

## New Core Structure

The core of the bot will be completely redisigned this includes:
 - **The only** Commands that still reside in the Core (aka Miku.DiscordBot) Project are Dev Commands, that are used for loading and unloading Commandmodules
 - All commands move to seperate Projects
 - Music has the least priority and will start by using Lavalink again, and later will move to VoiceNext for more compatibility with specific sites (NicoNicoDouga, BiliBili etc.)

If you're interested in how the "hotloading/unloading" of commands will work, here's an example project, or rather the base for the rewrite, which  will soon be comitted here in this new Repo
[DemoProject](https://cdn.discordapp.com/attachments/640169738106044467/686935549440819201/Miku.zip)

## IMPORTANT INFO
### Command modules should not have any dependencies on the Miku.DiscordBot project, same goes the other way around! No dependencies from Miku.DiscordBot to any Command Project 

##  Command Checklist

### Fun

 - [ ] 8Ball
 - [ ] Cat 🔵 
	 New library needed 
	 - [ ] [TheCatAPI](https://thecatapi.com/)
	 - [ ] [Cat as a service](https://cataas.com/#/)
	 - [ ] ... (or some other?)
 - [ ] ChuckNorris 🟠
 - [ ] Clapify
 - [ ] Clyde
	 - [ ] [NekoBot Api lib needed](https://docs.nekobot.xyz/#image-generation-clyde)
 - [ ] Coinflip
 - [ ] Dadjoke 🟠
 - [ ] Dog 🔵
 - [ ] Duck 🔵
 - [ ] Eyeify
 - [ ] Leet 🟠
 - [ ] Lion 🔵
 - [ ] Lizard 🔵
 - [ ] Panda 🔵
 - [ ] Penguin 🔵
 - [ ] Pirate
 - [ ] Red Panda 🔵
 - [ ] RPS (Rock Paper Scissors)
 - [ ] Tiger 🔵
 - [ ] Tiny
 - [ ] Trumptweet 🟠
	 - [ ] [DankMemer API](https://dankmemer.services/documentation) For more Image stuff

### General

 - [ ] Donate
 - [ ] Feedback
 - [ ] Help
 - [ ] Invite
 - [ ] Server/Support
 - [ ] Ping
 - [ ] Stats

### Moderation

 - [ ] Ban/UnBan
 - [ ] Kick
 - [ ] Purge (Messages)

### Utility

 - [ ] Anime
	 New library needed 
	 - [ ] [Jikan (MAL)](https://jikan.moe/)
	 - [ ] [Kitsu (Has good .Net package)](https://kitsu.docs.apiary.io/#introduction/json-api)
 - [ ] Avatar
 - [ ] Emoji List
 - [ ] Guild Info/Server Info
 - [ ] Manga (via Jikan or Kitsu, see above)
 - [ ] User Info
 - [ ] VocaDB
 - [ ] TouhouDB
 - [ ] UtaiteDB 

### Weeb 🔵 (Maybe rename to Image category?)

 - [ ] Awooify 🔵 Move to Fun? (also Nekobot API)
 - [ ] (All the Meek.Moe image API endpoints)
 - [ ] (All of Derpy's API endpoints)

### Action

 - [ ] Hug
	 - [ ] [Weeb.Sh needed (maybe)](https://docs.weeb.sh/) Perhaps get new API key with more features
 - [ ] Kiss (Same as above)
 - [ ] Lick (Same as above)
 - [ ] Pat (Same as above)
 - [ ] Poke (Same as above)
 - [ ] Slap (Same as above)
 - [ ] ... (maybe some more?)

### NSFW

 - [ ] ... (The current, but we need new libraries for that and I cant make those at work lol)
	 New Libraries needed
	 - [ ] [Nekos.Life](https://nekos.life/api/v2/endpoints)
	 - [ ] [KSoft](https://api.ksoft.si/) We already have an API key
	 - [ ] Danbooru 
	 - [ ] Gelbooru?
	 - [ ] Rule34?

##
🟠 = Perhaps unfitting for the bot
🔵 = Maybe better fitting in a seperate Category 

