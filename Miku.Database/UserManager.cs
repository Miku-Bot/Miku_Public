using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Miku.Database.UserEntities;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Threading.Tasks;

namespace Miku.Database
{
    public class UserManager : DbContext
    {
        private DbSet<User> Users { get; set; }
        private DbSet<UserPrefix> UserPrefixes { get; set; }

        public UserManager(DbContextOptions<UserManager> config) : base(config)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserPrefix>(entity =>
            {
                entity.HasKey(x => new
                {
                    x.Id,
                    x.UserId
                });
                entity.HasOne(x => x.AttachedUser)
                    .WithMany(x => x.Prefixes)
                    .IsRequired();
                entity.Property(x => x.Prefix)
                    .IsRequired()
                    .IsUnicode();
                entity.Property(x => x.AllowDefaultPrefix)
                    .IsRequired()
                    .HasDefaultValue(false);
            });
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasMany(x => x.Prefixes)
                    .WithOne(x => x.AttachedUser)
                    .HasForeignKey(x => x.UserId);
                entity.Property(x => x.MikuTranslator)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(false);
                entity.Property(x => x.MikuContributor)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(false);
                entity.Property(x => x.MikuDeveloper)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(false);
                entity.Property(x => x.LanguageContributions)
                    .HasConversion(
                        y => string.Join(',', y),
                        y => y.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                entity.Property(x => x.SetLanguage)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue("en-US");
                entity.Property(x => x.Created)
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                entity.Property(x => x.Modified)
                    .IsRequired()
                    .ValueGeneratedOnAddOrUpdate();
            });
        }

        public async Task<User> GetOrAddUserAsync(ulong id)
        {
            if (!await Users.AnyAsync(x => x.Id == id))
            {
                Users.Add(new User
                {
                    Id = id,
                    Created = DateTimeOffset.UtcNow,
                    Modified = DateTimeOffset.UtcNow
                });
                await SaveChangesAsync();
                Console.WriteLine("Added User: " + id);
            }

            return await Users.FirstAsync(x => x.Id == id);
        }

        public async Task UpdateUserAsync(User user)
        {
            Users.Update(user);
            await SaveChangesAsync();
        }
    }
}
