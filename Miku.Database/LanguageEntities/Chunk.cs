using System;
using System.Collections.Generic;
using System.Text;

namespace Miku.Database.LanguageEntities
{
    public class Chunk
    {
        public PropertyType Type { get; set; }
        public string Name { get; set; }
        public string InnerIdentifier { get; set; }
        public string LanguageCode { get; set; }
        public Language Language { get; set; }
        public string Text { get; set; }
        public List<Abbreviation> Abbreviations { get; set; }
    }
}
