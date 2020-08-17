using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Miku.Database.LanguageEntities;

namespace Miku.Database
{
    public class LanguageManager : DbContext
    {
        private DbSet<Language> Languages { get; set; }
        private DbSet<Chunk> Chunks { get; set; }
        private DbSet<Abbreviation> Abbreviations { get; set; }

        public LanguageManager(DbContextOptions<LanguageManager> config) : base(config)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Abbreviation>(entity =>
            {
                entity.HasKey(x => new
                {
                    x.Name,
                    x.Position,
                    x.ParentType,
                    x.ParentName,
                    x.ParentIdentifier,
                    x.ParentLanguageCode,
                });
                entity.HasOne(x => x.Parent)
                    .WithMany(x => x.Abbreviations)
                    .HasForeignKey(x => new
                    {
                        x.ParentType,
                        x.ParentName,
                        x.ParentIdentifier,
                        x.ParentLanguageCode
                    });
                entity.Property(x => x.Description)
                    .IsRequired()
                    .IsUnicode();
            });
            modelBuilder.Entity<Chunk>(entity =>
            {
                entity.HasKey(x => new
                {
                    x.Type,
                    x.Name,
                    x.InnerIdentifier,
                    x.LanguageCode
                });
                entity.HasMany(x => x.Abbreviations)
                    .WithOne(x => x.Parent)
                    .HasForeignKey(x => new
                    {
                        x.ParentType,
                        x.ParentName,
                        x.ParentIdentifier,
                        x.ParentLanguageCode
                    });
                entity.HasOne(x => x.Language)
                    .WithMany(x => x.TranslatedChunks)
                    .HasForeignKey(x => x.LanguageCode);
                entity.Property(x => x.Text)
                    .IsRequired()
                    .IsUnicode();
            });
            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasKey(x => x.Code);
                entity.HasMany(x => x.TranslatedChunks)
                    .WithOne(x => x.Language)
                    .HasForeignKey(x => x.LanguageCode);
                entity.Property(x => x.FullName);
            });

        }


        public Language GetDefaultLanguage()
        {
            return Languages.First(x => x.Code == "en-US");
        }

        public async Task<Dictionary<string, Chunk>> GetChunkDictionaryAsync(string languageCode,
            PropertyType propertyType, string name)
        {
            var hm = await Chunks.Where(x => x.LanguageCode == languageCode && x.Type == propertyType && x.Name == name)
                .Include(x => x.Abbreviations).ToListAsync();
            var hm2 = await Chunks.Where(x => x.LanguageCode == "en-US" && x.Type == propertyType && x.Name == name)
                .Include(x => x.Abbreviations).ToListAsync();
            if (hm.Count == hm2.Count) return hm.ToDictionary(x => x.InnerIdentifier);
            foreach (var c in hm2)
            {
                if (!(hm.Any(x => x.InnerIdentifier == c.InnerIdentifier)))
                    hm.Add(c);
            }

            return hm.ToDictionary(x => x.InnerIdentifier);
        }

        public async Task<List<Language>> GetLanguagesAsync()
        {
            var langs = await Languages.ToListAsync();
            var ordered = langs.OrderBy(x => x.Code).ToList();
            return ordered;
        }

        public async Task<List<Chunk>> GetChunksAsync(string languageCode) 
            => await Chunks.Where(x => x.LanguageCode == languageCode).ToListAsync();

        public async Task<List<Chunk>> GetChunksAsync(string languageCode, PropertyType propertyType)
            => await Chunks.Where(x => x.LanguageCode == languageCode && x.Type == propertyType).ToListAsync();

        public async Task<List<Chunk>> GetChunksAsync(string languageCode, PropertyType propertyType, string propertyName)
            => await Chunks.Where(x => x.LanguageCode == languageCode && x.Name == propertyName && x.Type == propertyType)
                .Include(x => x.Abbreviations).ToListAsync();

        public async Task<Chunk> GetOrAddSpecificChunk(string languageCode, PropertyType propertyType, string propertyName, string innerIdentifier)
        {
            var result = await Chunks
                .Include(x => x.Abbreviations)
                .FirstOrDefaultAsync(x =>
                    x.LanguageCode == languageCode &&
                    x.Name == propertyName &&
                    x.Type == propertyType &&
                    x.InnerIdentifier == innerIdentifier);
            if (result != null) return result;

            result = await Chunks
                .Include(x => x.Abbreviations)
                .FirstOrDefaultAsync(x =>
                    x.LanguageCode == "en-US" &&
                    x.Type == propertyType &&
                    x.Name == propertyName &&
                    x.InnerIdentifier == innerIdentifier);
            var otherlang = await Languages.FirstOrDefaultAsync(x => x.Code == languageCode);
            var newChunk = new Chunk
            {
                Abbreviations = new List<Abbreviation>(),
                Name = propertyName,
                InnerIdentifier = innerIdentifier,
                Language = otherlang,
                LanguageCode = languageCode,
                Type = propertyType,
                Text = "Translated Text Here " + languageCode
            };
            for (var i = 0; i < result.Abbreviations.Count; i++)
            {
                var abbr = result.Abbreviations[i];
                newChunk.Abbreviations.Add(new Abbreviation
                {
                    Description = abbr.Description,
                    Name = abbr.Name,
                    Parent = newChunk,
                    ParentType = newChunk.Type,
                    ParentIdentifier = newChunk.InnerIdentifier,
                    ParentName = newChunk.Name,
                    ParentLanguageCode = otherlang.Code,
                    Position = abbr.Position
                });
            }
            Chunks.Add(newChunk);
            await SaveChangesAsync();
            return newChunk;
        }

        public async Task<List<Abbreviation>> GetAllAbbreviationsAsync()
            => await Abbreviations.ToListAsync();

        public async Task AddChunkAsync(string languageCode, Chunk newChunk, List<Abbreviation> abbreviations)
        {
            var thatLang = await Languages.FirstAsync(x => x.Code == languageCode);
            newChunk.Language = thatLang;
            newChunk.LanguageCode = thatLang.Code;
            Chunks.Add(newChunk);
            for (var i = 0; i < abbreviations.Count; i++)
            {
                Abbreviations.Add(new Abbreviation
                {
                    Parent = newChunk,
                    ParentIdentifier = newChunk.InnerIdentifier,
                    ParentName = newChunk.Name,
                    ParentLanguageCode = newChunk.LanguageCode,
                    ParentType = newChunk.Type,
                    Description = abbreviations[i].Description,
                    Name = abbreviations[i].Name,
                    Position = abbreviations[i].Position
                });
            }
            await SaveChangesAsync();
        }

        public async Task UpdateChunkAsync(Chunk newChunk, List<Abbreviation> abbreviations = null)
        {
            try
            {
                var old = await Chunks.FirstAsync(x =>
                    x.Type == newChunk.Type && x.Name == newChunk.Name &&
                    x.InnerIdentifier == newChunk.InnerIdentifier && x.LanguageCode == newChunk.LanguageCode);
                old.Text = newChunk.Text;
                Chunks.Update(old);
                await SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
