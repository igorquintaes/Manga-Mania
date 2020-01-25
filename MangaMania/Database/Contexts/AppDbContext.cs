using MangaMania.Entities;
using Microsoft.EntityFrameworkCore;

namespace MangaMania.Database.Contexts
{
    public class AppDbContext : DbContext
    {
        private const int MAX_FIELD_LENGTH = 1000;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Manga>(buildAction =>
            {
                buildAction.Property(b => b.Name).IsRequired().HasMaxLength(MAX_FIELD_LENGTH);
                buildAction.Property(b => b.Author).IsRequired().HasMaxLength(MAX_FIELD_LENGTH);

                buildAction.HasMany(x => x.ScanlatorManga)
                    .WithOne(x => x.Manga)
                    .HasForeignKey(x => x.MangaId);

                buildAction.HasMany(x => x.Chapters)
                    .WithOne(x => x.Manga)
                    .HasForeignKey(x => x.MangaId);
            });

            modelBuilder.Entity<Scanlator>(buildAction =>
            {
                buildAction.Property(b => b.Name).IsRequired().HasMaxLength(MAX_FIELD_LENGTH);
                buildAction.Property(b => b.WebSite).IsRequired().HasMaxLength(MAX_FIELD_LENGTH);

                buildAction.HasMany(x => x.ScanlatorManga)
                    .WithOne(x => x.Scanlator)
                    .HasForeignKey(x => x.ScanlatorId);

                buildAction.HasMany(x => x.Chapters)
                    .WithOne(x => x.Scanlator)
                    .HasForeignKey(x => x.ScanlatorId);
            });

            modelBuilder.Entity<Chapter>(buildAction =>
            {
                buildAction.Property(b => b.Name).IsRequired().HasMaxLength(MAX_FIELD_LENGTH);
                buildAction.Property(b => b.Number).IsRequired();

                buildAction.HasOne(x => x.Manga)
                    .WithMany(x => x.Chapters)
                    .HasForeignKey(x => x.MangaId);

                buildAction.HasOne(x => x.Scanlator)
                    .WithMany(x => x.Chapters)
                    .HasForeignKey(x => x.ScanlatorId);
            });
        }

        public DbSet<Manga> Mangas { get; set; }
        public DbSet<Scanlator> Scanlators { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
    }
}
