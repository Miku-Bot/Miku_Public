using System;
using System.Collections.Generic;
using System.Text;

namespace Miku.Database.MusicEntities
{
    public class MusicGuildSettings
    {
        public ulong GuildId { get; set; }
        public MusicGuild Guild { get; set; }
        public Repeat Repeat { get; set; }
        public Shuffle Shuffle { get; set; }
        public RadioMode RadioMode { get; set; }
        public MusicHandler Handler { get; set; }
        public IList<ulong> AllowedRoles { get; set; }
        public bool DisableResultDeletion { get; set; }
    }
}
