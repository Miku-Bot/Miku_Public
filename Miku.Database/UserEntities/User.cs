using Miku.Database.LanguageEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miku.Database.UserEntities
{
    public class User
    {
        public ulong Id { get; set; }
        public bool MikuTranslator { get; set; }
        public bool MikuContributor { get; set; }
        public bool MikuDeveloper { get; set; }
        public IList<string> LanguageContributions { get; set; }
        public string SetLanguage { get; set; }
        public IList<UserPrefix> Prefixes { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }

    }
}
