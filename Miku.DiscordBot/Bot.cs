using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.VoiceNext;
using Ionic.Zip;
using Meek.Moe.Weeb.Sh;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Miku.Core;
using Miku.Core.ConfigEntities;
using Miku.Database;
using Miku.Database.UserEntities;
using Miku.DiscordBot.CoreCommands;
using Miku.DiscordBot.Util;

namespace Miku.DiscordBot
{
    public class Bot : IAsyncDisposable, IDisposable
    {
        private HttpClient httpClient { get; set; } = new HttpClient();
        private Utility utility { get; set; } = new Utility();
        private DiscordClient discordClient { get; set; }
        private CommandsNextExtension commandsNextExtension { get; set; }
        private InteractivityExtension interactivityExtension { get; set; }
        private VoiceNextExtension voiceNextExtension { get; set; }

        private Dictionary<string, AsLoader> LoadedCommands { get; set; } = new Dictionary<string, AsLoader>();

        public Bot(BotConfig botConfig, DBConfig dBConfig)
        {
            DiscordConfiguration discordConfiguration = new DiscordConfiguration
            {
                Token = botConfig.Token,
                TokenType = TokenType.Bot,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true,
                ShardCount = 1,
                ShardId = 0
            };
            ServiceProvider deps = new ServiceCollection()
                .AddSingleton(httpClient)
                .AddSingleton(utility)
                .AddSingleton(new WeebShAPIClient(botConfig.WeebSHToken, httpClient))
                .AddDbContext<LanguageManager>(x => x.UseNpgsql(dBConfig.ConnString), ServiceLifetime.Transient)
                .AddDbContext<UserManager>(x => x.UseNpgsql(dBConfig.ConnString), ServiceLifetime.Transient)
                .BuildServiceProvider();
            CommandsNextConfiguration commandsNextConfiguration = new CommandsNextConfiguration
            {
                StringPrefixes = new []{ botConfig.BetaPrefix },
                Services = deps
            };
            InteractivityConfiguration interactivityConfiguration = new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.WrapAround,
                PaginationDeletion = PaginationDeletion.DeleteEmojis,
                PollBehaviour = PollBehaviour.DeleteEmojis
            };
            VoiceNextConfiguration voiceNextConfiguration = new VoiceNextConfiguration
            {
                AudioFormat = AudioFormat.Default
            };
            this.discordClient = new DiscordClient(discordConfiguration);
            this.commandsNextExtension = this.discordClient.UseCommandsNext(commandsNextConfiguration);
            this.interactivityExtension = this.discordClient.UseInteractivity(interactivityConfiguration);
            this.voiceNextExtension = this.discordClient.UseVoiceNext(voiceNextConfiguration);
            this.discordClient.ClientErrored += e =>
            {;
                Console.WriteLine("Client Error:");
                Console.WriteLine(e.EventName);
                Console.WriteLine(e.Exception);
                return Task.CompletedTask;
            };
            this.discordClient.SocketErrored += e =>
            {
                Console.WriteLine("Socket Error:");
                Console.WriteLine(e.Exception);
                return Task.CompletedTask;
            };
            this.discordClient.GuildDownloadCompleted += e =>
            {
                this.commandsNextExtension.RegisterCommands<Dev>();
                foreach (var f in Directory.EnumerateFiles(@"Commands\\"))
                {
                    using (ZipFile zip = ZipFile.Read(@$"{f}"))
                    {
                        var dll = zip.First(x => x.FileName.EndsWith(".dll"));
                        var splitFileName = dll.FileName.Split('.');
                        var group = splitFileName[^2];
                        if (LoadedCommands.ContainsKey(group)) continue;
                        LoadedCommands.Add(group, new AsLoader());
                        using (var hm = dll.OpenReader())
                        {
                            var assembly = LoadedCommands[group].LoadFromStream(hm);
                            commandsNextExtension.RegisterCommands(assembly);
                        }
                    }
                }
                return Task.CompletedTask;
            };
            this.commandsNextExtension.CommandErrored += e =>
            {
                Console.WriteLine(e.Exception);
                return Task.CompletedTask;
            };
            //this.commandsNextExtension.CommandExecuted += async e =>
            //{
            //    //var d = new UserManager();
            //    //var u = await d.GetOrAddUserAsync(e.Context.User.Id);
            //    //Console.WriteLine($"User {e.Context.User} executed {e.Command.Name}, Language {u.SetLanguage}");
            //};
        }

        public async ValueTask StartBotTask()
        {
            DiscordActivity discordActivity = new DiscordActivity("Testing", ActivityType.Watching);
            await this.discordClient.ConnectAsync(discordActivity, UserStatus.Online);
            while (true) await Task.Delay(100);
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
