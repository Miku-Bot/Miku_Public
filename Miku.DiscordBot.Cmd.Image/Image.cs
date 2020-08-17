using System;
using System.Dynamic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Miku.Core;

namespace Miku.DiscordBot.Cmd.Image
{
    public class Image : BaseCommandModule
    {
        private HttpClient httpClient { get; }

        public Image(HttpClient client)
        {
            httpClient = client;
        }
    }
}
