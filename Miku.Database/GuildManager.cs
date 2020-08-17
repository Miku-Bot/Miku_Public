using Microsoft.EntityFrameworkCore;
using Miku.Database.GuildEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Miku.Database
{
    public class GuildManager : DbContext
    {
        private DbSet<Guild> Guilds{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Guild>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Prefix)
                    .IsRequired()
                    .IsUnicode();
                entity.Property(x => x.AllowDefaultPrefix)
                    .IsRequired()
                    .HasDefaultValue(true);
                entity.Property(x => x.AllowMentionPrefix)
                    .IsRequired()
                    .HasDefaultValue(true);
                entity.Property(x => x.DisableUserPrefix)
                    .IsRequired()
                    .HasDefaultValue(false);
                entity.Property(x => x.DisabledCommands)
                    .IsRequired()
                    .HasConversion(x => string.Join(",", x),
                    x => x.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
            });
        }
    }
}
