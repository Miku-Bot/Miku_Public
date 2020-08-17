using System;
using System.Threading.Tasks;

namespace Miku.Database.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var l = new LanguageManager();
            await l.GetOrAddSpecificChunk("nl-NL", "stats", "embed_guildcountfield_description");
        }
    }
}
