using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Meek.Moe.Weeb.Sh;
using Miku.Core;
using Miku.Core.Extensions;
using Miku.Database;

namespace Miku.DiscordBot.Cmd.Action
{
    public class Action : BaseCommandModule
    {
        private HttpClient httpClient { get; }
        private Utility utility { get; set; }
        private LanguageManager languageManager { get; }
        private UserManager userManager { get; }
        private WeebShAPIClient weebShAPIClient { get; }

        public Action(HttpClient client, Utility util, LanguageManager langManager, UserManager usManager)
        {
            httpClient = client;
            utility = util;
            languageManager = langManager;
            userManager = usManager;
        }

        [Command("hug")]
        [Priority(2)]
        [Description("Hug someone!")]
        public async Task Hug(CommandContext ctx)
        {
            var user = await userManager.GetOrAddUserAsync(ctx.User.Id);
            var texts = await ctx.GetLanguageTexts(user.SetLanguage);
            var got = await weebShAPIClient.GetRandomImageAsync("hug", null);
            var emb = new DiscordEmbedBuilder();
            emb.WithTitle(texts["embed_title"].Replace());
            emb.WithDescription(texts["embed_description:self"].Replace(ctx.Member.Mention));
            emb.WithImageUrl(got.Url);
            await ctx.RespondAsync(embed: emb.Build());
        }

        [Command("hug")]
        [Priority(1)]
        public async Task Hug(CommandContext ctx, params DiscordUser[] mentioned)
        {
            if (mentioned[0] == ctx.Message.Author)
            {
                await Hug(ctx);
                return;
            }
            var user = await userManager.GetOrAddUserAsync(ctx.User.Id);
            var texts = await ctx.GetLanguageTexts(user.SetLanguage);
            var got = await weebShAPIClient.GetRandomImageAsync("hug", null);
            var emb = new DiscordEmbedBuilder();
            emb.WithTitle(texts["embed_title"].Replace());
            if (mentioned.Length == 1)
                emb.WithDescription(texts["embed_description:single"].Replace(ctx.Member.Mention, mentioned.Select(x => x.Mention).First()));
            else
                emb.WithDescription(texts["embed_description:multi"].Replace(ctx.Member.Mention, mentioned.Select(x => x.Mention).ToArray()));
            emb.WithImageUrl(got.Url);
            await ctx.RespondAsync(embed: emb.Build());
        }
    }
}
