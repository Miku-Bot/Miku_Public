using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Miku.Core;
using Miku.Core.Extensions;
using Miku.Database;

namespace Miku.DiscordBot.Cmd.General
{
    public class General : BaseCommandModule
    {
        private HttpClient httpClient { get; }
        private Utility utility { get; set; }
        private LanguageManager languageManager { get; }
        private UserManager userManager { get; }
        
        public General(HttpClient client, Utility util, LanguageManager langManager, UserManager usManager)
        {
            httpClient = client;
            utility = util;
            languageManager = langManager;
            userManager = usManager;
        }
        
        [Command("donate")]
        public async Task DonateAsync(CommandContext ctx)
        {
            var user = await userManager.GetOrAddUserAsync(ctx.User.Id);
            var texts = await ctx.GetLanguageTexts(user.SetLanguage);
            var emb = new DiscordEmbedBuilder();
            emb.WithTitle(texts["embed_title"].Replace());
            emb.WithDescription(texts["embed_description"].Replace());
            emb.AddField("_____", "[Patreon](https://patreon.com/speyd3r)", true);
            emb.AddField("_____", "[PayPal](https://paypal.me/speyd3r)", true);
            emb.WithThumbnail(ctx.Client.CurrentUser.AvatarUrl);
            await ctx.RespondAsync(embed: emb.Build());
        }

        [Command("invite")]
        public async Task InviteAsync(CommandContext ctx)
        {
            var user = await userManager.GetOrAddUserAsync(ctx.User.Id);
            var texts = await ctx.GetLanguageTexts(user.SetLanguage);
            await ctx.RespondAsync(texts["text"].Replace() + $"\nhttps://meek.moe/miku");
        }
        
        [Command("ping")]
        [Cooldown(1,5,CooldownBucketType.User)]
        public async Task PingAsync(CommandContext ctx)
        {
            var user = await userManager.GetOrAddUserAsync(ctx.User.Id);
            var texts = await ctx.GetLanguageTexts(user.SetLanguage);
            var emb = new DiscordEmbedBuilder();
            emb.WithTitle(texts["embed_title"].Replace());
            emb.WithDescription(texts["embed_description"].Replace(ctx.Client.Ping));
            await ctx.RespondAsync(embed: emb.Build());
        }
        
        [Command("stats")]
        public async Task StatsAsync(CommandContext ctx)
        {
            var user = await userManager.GetOrAddUserAsync(ctx.User.Id);
            var texts = await ctx.GetLanguageTexts(user.SetLanguage);
            ulong userCount = ctx.Client.Guilds.Aggregate<KeyValuePair<ulong, DiscordGuild>, ulong>(0, (current, guild) => current + Convert.ToUInt64(guild.Value.MemberCount));
            ulong channelCount = ctx.Client.Guilds.Aggregate<KeyValuePair<ulong, DiscordGuild>, ulong>(0, (current, guild) => current + Convert.ToUInt64(guild.Value.Channels.Count));
            string botDevs = ctx.Client.CurrentApplication.Owners.Aggregate("", (current, dev) => current + $"{dev.Username}#{dev.Discriminator}, ");
            var emb = new DiscordEmbedBuilder();
            emb.WithTitle(texts["embed_title"].Replace());
            emb.WithThumbnail(ctx.Client.CurrentUser.AvatarUrl);
            emb.AddField(texts["embed_shardIdField_title"].Replace(),
                texts["embed_shardIdField_description"].Replace(ctx.Client.ShardId, ctx.Client.ShardCount));
            emb.AddField(texts["embed_guildCountField_title"].Replace(),
                texts["embed_guildCountField_description"].Replace(ctx.Client.Guilds.Count),true);
            emb.AddField(texts["embed_channelCountField_title"].Replace(),
                texts["embed_channelCountField_description"].Replace(channelCount));
            emb.AddField(texts["embed_userCountField_title"].Replace(), 
                texts["embed_userCountField_description"].Replace(userCount),true);
            emb.AddField(texts["embed_botDevsField_title"].Replace(), 
                texts["embed_botDevsField_description"].Replace(botDevs));
            emb.AddField(texts["embed_yourLanguageField_title"].Replace(),
                texts["embed_yourLanguageField_description"].Replace("TBA_LangName", "TBA_LangCode"), true);
            emb.AddField(texts["embed_yourLanguageContributorsField_title"].Replace(),
                texts["embed_yourLanguageContributorsField_description"].Replace("TBA_Contributors"));
            emb.AddField(texts["embed_guildLanguageField_title"].Replace(),
                texts["embed_guildLanguageField_description"].Replace("TBA_LangName", "TBA_LangCode"), true);
            emb.AddField(texts["embed_guildLanguageContributorsField_title"].Replace(),
                texts["embed_guildLanguageContributorsField_description"].Replace("TBA_Contributors"));
            emb.AddField(texts["embed_allLanguageContributorsField_title"].Replace(),
                texts["embed_allLanguageContributorsField_description"].Replace("TBA_Contributors"), true);
            emb.AddField(texts["embed_pingField_title"].Replace(), 
                texts["embed_pingField_description"].Replace(ctx.Client.Ping));
            await ctx.RespondAsync(embed: emb.Build());
        }
        
        [Command("support")]
        public async Task SupportAsync(CommandContext ctx)
        {
            var user = await userManager.GetOrAddUserAsync(ctx.User.Id);
            var texts = await ctx.GetLanguageTexts(user.SetLanguage);
            var emb = new DiscordEmbedBuilder();
            emb.WithThumbnail(ctx.Client.CurrentUser.AvatarUrl);
            emb.WithTitle(texts["embed_title"].Replace());
            emb.WithDescription(texts["embed_description"]
                .Replace($"https://discord.gg/YPPA2Pu"));
            await ctx.RespondAsync(embed: emb.Build());
        }
    }
}
