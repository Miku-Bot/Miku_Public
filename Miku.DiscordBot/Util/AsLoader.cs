using System;
using System.Reflection;
using System.Runtime.Loader;

namespace Miku.DiscordBot.Util
{
    public class AsLoader : AssemblyLoadContext
    {
        public AsLoader() : base(true)
        {
        }
    }
}
