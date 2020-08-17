using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Miku.Database.LanguageEntities
{
    public class Language
    {
        public string Code { get; set; } //Key
        public string FullName { get; set; }
        public IList<Chunk> TranslatedChunks { get; set; }

    }
}
