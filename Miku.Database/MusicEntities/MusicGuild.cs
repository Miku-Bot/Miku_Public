using Miku.Database.GuildEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miku.Database.MusicEntities
{
    public class MusicGuild
    {
        public ulong GuildId { get; set; }
        public MusicGuildSettings Settings { get; set; }
        public Status Status { get; set; }
        public IList<QueueEntry> Queue { get; set; }
        public DateTimeOffset StartedListeningAt { get; set; }

    }
}
