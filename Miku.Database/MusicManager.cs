using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Miku.Database.MusicEntities;

namespace Miku.Database
{
    public class MusicManager : DbContext
    {
        private DbSet<MusicGuild> MusicGuilds { get; set; }
        private DbSet<MusicGuildSettings> MusicGuildSettings { get; set; }
        private DbSet<QueueEntry> QueueEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QueueEntry>(entity =>
            {
                entity.HasKey(x => new
                {
                    x.GuildId,
                    x.Position
                });
                entity.HasOne(x => x.Guild)
                    .WithMany(x => x.Queue)
                    .HasForeignKey(x => x.GuildId)
                    .IsRequired();
                entity.Property(x => x.AddedAt)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(DateTimeOffset.UtcNow)
                    .IsRequired();
                entity.Property(x => x.AddedBy)
                    .IsRequired();
                entity.Property(x => x.TrackString)
                    .IsRequired()
                    .IsUnicode();
            });

            modelBuilder.Entity<MusicGuildSettings>(entity =>
            {
                entity.HasKey(x => x.GuildId);
                entity.HasOne(x => x.Guild)
                    .WithOne(x => x.Settings)
                    .HasForeignKey<MusicGuildSettings>(x => x.GuildId);
                entity.Property(x => x.Repeat)
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(Repeat.Off);
                entity.Property(x => x.Shuffle)
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(Shuffle.Off);
                entity.Property(x => x.Handler)
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(MusicHandler.Legacy);
                entity.Property(x => x.RadioMode)
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(RadioMode.Off);
                entity.Property(x => x.DisableResultDeletion)
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(false);
                entity.Property(x => x.AllowedRoles)
                    .IsRequired()
                    .HasConversion(x => string.Join(',', x),
                        x => ConvertStringToUlongList(x));
            });

            modelBuilder.Entity<MusicGuild>(entity =>
            {
                entity.HasKey(x => x.GuildId);
                entity.HasOne(x => x.Settings)
                    .WithOne(x => x.Guild)
                    .HasForeignKey<MusicGuild>(x => x.GuildId);
                entity.HasMany(x => x.Queue)
                    .WithOne(x => x.Guild)
                    .HasForeignKey(x => x.GuildId)
                    .IsRequired();
                entity.Property(x => x.Status)
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(Status.Stopped);
                entity.Property(x => x.StartedListeningAt)
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(DateTimeOffset.UtcNow);
            });

            
        }

        private List<ulong> ConvertStringToUlongList(string input)
        {
            var splits = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var result = new List<ulong>();
            foreach (var role in splits)
                result.Add(ulong.Parse(role));
            return result;
        }
    }
}
