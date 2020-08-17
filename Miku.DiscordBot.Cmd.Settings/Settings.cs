using Miku.Core;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Miku.Database;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Miku.Core.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace Miku.DiscordBot.Cmd.Settings
{
    [Group()]
    [Aliases("config", "configuration", "c","s", "set")]
    public class Settings : BaseCommandModule
    {
        private HttpClient httpClient { get; }
        private Utility utility { get; set; }
        private LanguageManager languageManager { get; }
        private UserManager userManager { get; }


        public Settings(HttpClient client, Utility util, LanguageManager langManager, UserManager usManager)
        {
            httpClient = client;
            utility = util;
            languageManager = langManager;
            userManager = usManager;
        }

        [Command("setlanguage")]
        [Aliases("setlang", "sl")]
        [Priority(2)]
        public async Task SetLanguageAsync(CommandContext ctx)
        {
            var inter = ctx.Client.GetInteractivity();
            var user = await userManager.GetOrAddUserAsync(ctx.User.Id);
            var texts = await ctx.GetLanguageTexts(user.SetLanguage);
            var alllangs = await languageManager.GetLanguagesAsync();
            var emb = new DiscordEmbedBuilder();
            emb.WithTitle(texts["selectEmbed_title"].Replace());
            string langlist = alllangs.Aggregate("", (current, lang) => current + $"{lang.Code} - {lang.FullName}\n");
            emb.WithDescription(texts["selectEmbed_description"].Replace(langlist));
            emb.AddField(texts["selectEmbed_howToField_title"].Replace(),
                texts["selectEmbed_howToField_description"].Replace());
            var selectMsg = await ctx.RespondAsync(embed: emb.Build());
            var response = await inter.WaitForMessageAsync(x => x.Author == ctx.User, TimeSpan.FromSeconds(60));
            if (response.TimedOut)
            {
                await selectMsg.DeleteAsync();
                await ctx.RespondAsync(texts["timeOut_text"].Replace());
                return;
            }
            else if (alllangs.All(x => x.Code.ToLower() != response.Result.Content.ToLower()))
            {
                await selectMsg.DeleteAsync();
                await ctx.RespondAsync(texts["invalidCode_text"].Replace());
                return;
            }

            await selectMsg.DeleteAsync();
            var selectedlang = alllangs.First(x => x.Code.ToLower() == response.Result.Content.ToLower());
            user.SetLanguage = selectedlang.Code;
            await userManager.UpdateUserAsync(user);
            emb = new DiscordEmbedBuilder();
            emb.WithTitle(texts["successEmbed_title"].Replace());
            emb.WithDescription(texts["successEmbed_description"].Replace(selectedlang.Code, selectedlang.FullName));
            await ctx.RespondAsync(embed: emb.Build());
        }

        [Command("setlanguage")]
        [Priority(1)]
        public async Task SetLanguageAsync(CommandContext ctx, string langcode)
        {
            var user = await userManager.GetOrAddUserAsync(ctx.User.Id);
            var texts = await ctx.GetLanguageTexts(user.SetLanguage);
            var alllangs = await languageManager.GetLanguagesAsync();
            if (alllangs.All(x => x.Code.ToLower() != langcode.ToLower()))
            {
                await ctx.RespondAsync(texts["invalidCode_text"].Replace());
                return;
            }
            var selectedlang = alllangs.First(x => x.Code.ToLower() == langcode.ToLower());
            user.SetLanguage = selectedlang.Code;
            await userManager.UpdateUserAsync(user);
            var emb = new DiscordEmbedBuilder();
            emb.WithTitle(texts["successEmbed_title"].Replace());
            emb.WithDescription(texts["successEmbed_description"].Replace(selectedlang.Code, selectedlang.FullName));
            await ctx.RespondAsync(embed: emb.Build());
        }

        [Command("listlanguages")]
        [Aliases("languages", "langs")]
        public async Task ListLanguagesAsync(CommandContext ctx)
        {
            var user = await userManager.GetOrAddUserAsync(ctx.User.Id);
            var texts = await ctx.GetLanguageTexts(user.SetLanguage);
            var langs = await languageManager.GetLanguagesAsync();
            string langlist = langs.Aggregate("", (current, lang) => current + $"{lang.Code} - {lang.FullName}\n");
            var emb = new DiscordEmbedBuilder();
            emb.WithTitle(texts["embed_title"].Replace());
            emb.WithDescription(texts["embed_description"].Replace(langlist));
            await ctx.RespondAsync(embed: emb.Build());
        }
    }
}
