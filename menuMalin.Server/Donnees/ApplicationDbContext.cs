using Microsoft.EntityFrameworkCore;
using menuMalin.Server.Modeles.Entites;

namespace menuMalin.Server.Donnees;

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
    public DbSet<Utilisateur> Utilisateurs { get; set; } = null!;
    public DbSet<Recette> Recettes { get; set; } = null!;
    public DbSet<Favori> Favoris { get; set; } = null!;
    public DbSet<RecetteUtilisateur> RecettesUtilisateur { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration Utilisateur
        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnType("varchar(36)");
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.DateCreation).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.HasIndex(e => e.Email).IsUnique();

            // Relations
            entity.HasMany(e => e.Favoris)
                .WithOne(f => f.Utilisateur)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Messages)
                .WithOne(c => c.Utilisateur)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuration Recette
        modelBuilder.Entity<Recette>(entity =>
        {
            entity.HasKey(e => e.RecipeId);
            entity.Property(e => e.RecipeId).HasColumnType("varchar(36)");
            entity.Property(e => e.Title).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Instructions).IsRequired();
            entity.Property(e => e.MealDBId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Area).HasMaxLength(100);
            entity.Property(e => e.Tags).HasMaxLength(500);
            entity.Property(e => e.DateCreation).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.Property(e => e.DateMaj)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                .ValueGeneratedOnAddOrUpdate();

            entity.HasIndex(e => e.MealDBId).IsUnique();
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Title);

            // Relations
            entity.HasMany(e => e.Favoris)
                .WithOne(f => f.Recette)
                .HasForeignKey(f => f.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration Favori
        modelBuilder.Entity<Favori>(entity =>
        {
            entity.HasKey(e => e.FavoriteId);
            entity.Property(e => e.FavoriteId).HasColumnType("varchar(36)");
            entity.Property(e => e.UserId).HasColumnType("varchar(36)").IsRequired();
            entity.Property(e => e.RecipeId).HasColumnType("varchar(36)").IsRequired();
            entity.Property(e => e.DateCreation).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.HasIndex(e => new { e.UserId, e.RecipeId }).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.RecipeId);

            // Relations
            entity.HasOne(e => e.Utilisateur)
                .WithMany(u => u.Favoris)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Recette)
                .WithMany(r => r.Favoris)
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration RecetteUtilisateur
        modelBuilder.Entity<RecetteUtilisateur>(entity =>
        {
            entity.HasKey(e => e.UserRecipeId);
            entity.Property(e => e.UserRecipeId).HasColumnType("varchar(36)");
            entity.Property(e => e.UserId).HasColumnType("varchar(36)").IsRequired();
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.IngredientsJson).HasColumnType("longtext");
            entity.Property(e => e.DateCreation).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.Property(e => e.DateMaj)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                .ValueGeneratedOnAddOrUpdate();

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsPublic);

            // Relations
            entity.HasOne(e => e.Utilisateur)
                .WithMany(u => u.RecettesUtilisateur)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration Message
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.ContactId);
            entity.Property(e => e.ContactId).HasColumnType("varchar(36)");
            entity.Property(e => e.UserId).HasColumnType("varchar(36)");
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Subject).HasMaxLength(500).IsRequired();
            entity.Property(e => e.MessageContenu).IsRequired();
            entity.Property(e => e.Statut).HasDefaultValue(StatutMessage.New);
            entity.Property(e => e.DateCreation).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.Property(e => e.DateMaj)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                .ValueGeneratedOnAddOrUpdate();

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.Statut);
            entity.HasIndex(e => e.DateCreation);
        });
    }
}