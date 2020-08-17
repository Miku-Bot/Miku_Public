using System;
using System.Collections.Generic;
using System.Text;

namespace Miku.Database.MusicEntities
{
    public class QueueEntry
    {
        public ulong GuildId { get; set; }
        public MusicGuild Guild { get; set; }
        public ulong Position { get; set; }
        public string TrackString { get; set; }
        public ulong AddedBy { get; set; }
        public DateTimeOffset AddedAt { get; set; }
    }
}
