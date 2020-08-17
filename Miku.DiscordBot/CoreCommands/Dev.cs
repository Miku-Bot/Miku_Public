using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using Ionic.Zip;
using Microsoft.EntityFrameworkCore;
using Miku.Database;
using Miku.DiscordBot.Util;

namespace Miku.DiscordBot.CoreCommands
{
    [RequireOwner]    
    public class Dev : BaseCommandModule
    {
        private LanguageManager languageManager { get; }

        public Dev(LanguageManager langManager)
        {
            languageManager = langManager;
        }

        [Command("register")]
        public async Task RegisterModule(CommandContext ctx, string name)
        {
            //if (Bot.test == null) Bot.test = new AsLoader();
            //using (ZipFile zip = ZipFile.Read(@"Commands\\Miku.DiscordBot.Cmd.General.1.0.0.nupkg"))
            //{
            //    var dll = zip.First(x => x.FileName.EndsWith(".dll"));
            //    using (var hm = dll.OpenReader())
            //    {
            //        var assembly =
            //            Bot.test.LoadFromStream(
            //                hm); // usual yada yada to get a Stream and let the Loader load the Assembly code
            //        ctx.CommandsNext.RegisterCommands(assembly); //Feed that to CommandsNext and let it add the commands
            //    }
            //}
            await ctx.RespondAsync("Registered");
        }

        [Command("unregister")]
        public async Task UnregisterModule(CommandContext ctx, string name)
        {
            //Bot.test.Unload();
            //Bot.test = null;
            //GC.Collect();
            //var cmds = //Get all commands that live in that Class we only have "Fun"
            //    ctx.CommandsNext.RegisteredCommands.Where(x => x.Value.Module.ModuleType.Name == name);
            //ctx.CommandsNext.UnregisterCommands(cmds.Select(x => x.Value).ToArray()); //Let CommandsNext unregister all those Commands
            await ctx.RespondAsync("Unregistered");
        }
    }
}
