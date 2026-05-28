using Microsoft.EntityFrameworkCore;
using MovieHub.API.Models;

namespace MovieHub.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Director> Directors => Set<Director>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Rating> Ratings => Set<Rating>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Username).HasMaxLength(50);
            entity.Property(u => u.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasIndex(g => g.Name).IsUnique();
            entity.Property(g => g.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Director>(entity =>
        {
            entity.Property(d => d.FirstName).HasMaxLength(50);
            entity.Property(d => d.LastName).HasMaxLength(50);
            entity.Property(d => d.Nationality).HasMaxLength(50);
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.Property(m => m.Title).HasMaxLength(150);
            entity.Property(m => m.Description).HasMaxLength(1000);
            entity.HasOne(m => m.Genre).WithMany(g => g.Movies).HasForeignKey(m => m.GenreId);
            entity.HasOne(m => m.Director).WithMany(d => d.Movies).HasForeignKey(m => m.DirectorId);
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.Property(r => r.Comment).HasMaxLength(500);
            entity.HasIndex(r => new { r.MovieId, r.UserId }).IsUnique();
            entity.HasOne(r => r.Movie).WithMany(m => m.Ratings).HasForeignKey(r => r.MovieId);
            entity.HasOne(r => r.User).WithMany(u => u.Ratings).HasForeignKey(r => r.UserId);
        });
    }
}
