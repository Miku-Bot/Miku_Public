using System;
using System.Collections.Generic;
using System.Text;

namespace Miku.Database.GuildEntities
{
    public class Guild
    {
        public ulong Id { get; set; }
        public string Prefix { get; set; }
        public string LanguageCode { get; set; }
        public bool ForceGuildLanguage { get; set; }
        public bool AllowDefaultPrefix { get; set; }
        public bool AllowMentionPrefix { get; set; }
        public bool DisableUserPrefix { get; set; }
        public IList<string> DisabledCommands { get; set; }
    }
}
