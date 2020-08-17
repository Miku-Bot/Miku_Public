using Miku.Core.ConfigEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Miku.Core
{
    public class Utility
    {
        public BotConfig LoadBotCfg()
        {
            var txt = File.ReadAllText(@"BotConfig.json");
            var bc = JsonSerializer.Deserialize<BotConfig>(txt);
            return bc;
        }
        public DBConfig LoadDBCfg()
        {
            var txt = File.ReadAllText(@"DBConfig.json");
            var bc = JsonSerializer.Deserialize<DBConfig>(txt);
            return bc;
        }
    }
}
