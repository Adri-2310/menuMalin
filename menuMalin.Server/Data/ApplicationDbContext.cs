using Microsoft.EntityFrameworkCore;
using menuMalin.Server.Models.Entities;

namespace menuMalin.Server.Data;

/// <summary>
/// DbContext pour l'application menuMalin
/// Gère le mapping entre les entités C# et la base de données MySQL
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets pour chaque entité
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Recipe> Recipes { get; set; } = null!;
    public DbSet<Favorite> Favorites { get; set; } = null!;
    public DbSet<ContactMessage> ContactMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnType("varchar(36)");
            entity.Property(e => e.Auth0Id).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.Auth0Id).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            // Relations
            entity.HasMany(e => e.Favorites)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.ContactMessages)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuration Recipe
        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId);
            entity.Property(e => e.RecipeId).HasColumnType("varchar(36)");
            entity.Property(e => e.Title).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Instructions).IsRequired();
            entity.Property(e => e.MealDBId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Area).HasMaxLength(100);
            entity.Property(e => e.Tags).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate();

            entity.HasIndex(e => e.MealDBId).IsUnique();
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Title);

            // Relations
            entity.HasMany(e => e.Favorites)
                .WithOne(f => f.Recipe)
                .HasForeignKey(f => f.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration Favorite
        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId);
            entity.Property(e => e.FavoriteId).HasColumnType("varchar(36)");
            entity.Property(e => e.UserId).HasColumnType("varchar(36)").IsRequired();
            entity.Property(e => e.RecipeId).HasColumnType("varchar(36)").IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => new { e.UserId, e.RecipeId }).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.RecipeId);
        });

        // Configuration ContactMessage
        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.ContactId);
            entity.Property(e => e.ContactId).HasColumnType("varchar(36)");
            entity.Property(e => e.UserId).HasColumnType("varchar(36)");
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Subject).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.Status).HasDefaultValue(ContactMessageStatus.New);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate();

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}