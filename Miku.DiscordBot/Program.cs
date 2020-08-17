using Miku.Core;
using System;
using System.Text.Json;

namespace Miku.DiscordBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var util = new Utility();
            var bc = util.LoadBotCfg();
            var dc = util.LoadDBCfg();
            using Bot bot = new Bot(bc, dc);
            bot.StartBotTask().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
