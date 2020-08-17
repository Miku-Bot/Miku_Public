using DSharpPlus.CommandsNext;
using Microsoft.Extensions.DependencyInjection;
using Miku.Database;
using Miku.Database.LanguageEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Miku.Core.Extensions
{
    public static class LanguageExtension
    {
        public static async Task<Dictionary<string, Chunk>> GetLanguageTexts(this CommandContext ctx, string lang)
        {
            LanguageManager lm = ctx.CommandsNext.Services.GetService<LanguageManager>();
            var cmds = await lm.GetChunkDictionaryAsync(lang, PropertyType.BotCommand, ctx.Command.Name);
            return cmds;
        }
        public static string Replace(this Chunk langtext, params object[] replacements)
        {
            var abbrs = langtext.Abbreviations.OrderBy(x => x.Position).ToList();
            for (var index = 0; index < abbrs.Count(); index++)
            {
                if (replacements[index] is Array multi && abbrs[index].Name.Contains(":multi"))
                {
                    var r = abbrs[index];
                    if (multi.Length == 0) continue;
                    string repl = ((object[]) multi)[0].ToString();
                    for (var i = 1; i < ((object[]) multi).Length; i++)
                    {
                        repl += ", " + ((object[]) multi)[i];
                    }

                    langtext.Text.Replace(r.Name, repl);
                }
                else
                {
                    var r = abbrs[index];
                    langtext.Text = langtext.Text.Replace(r.Name, replacements[index].ToString());
                }
            }

            return langtext.Text;
        }
    }
}
