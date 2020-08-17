using System;
using System.Collections.Generic;
using System.Text;

namespace Miku.Database.LanguageEntities
{
    public class Abbreviation
    {
        public string Name { get; set; }
        public int Position { get; set; }
        public string ParentName { get; set; }
        public string ParentIdentifier { get; set; }
        public PropertyType ParentType { get; set; }
        public string ParentLanguageCode { get; set; }
        public Chunk Parent { get; set; }
        public string Description { get; set; }
    }
}
