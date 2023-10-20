using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ViaRedditData");

        // Configuration for User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Password).HasColumnName("password_hash");

            entity.HasMany(e => e.Posts)
                .WithOne()
                .HasForeignKey(p => p.Username)
                .OnDelete(DeleteBehavior.Cascade); // Adjust as per your requirements

            entity.HasMany(e => e.Comments)
                .WithOne()
                .HasForeignKey(c => c.Username)
                .OnDelete(DeleteBehavior.Cascade); // Adjust as per your requirements
        });

        // Configuration for Post entity
        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("posts");
            entity.Property(e => e.PostID).HasColumnName("post_id");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Context).HasColumnName("context");

            entity.HasMany(e => e.Comments)
                .WithOne()
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust as per your requirements
        });

        // Configuration for Comment entity
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comments");
            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.CommentText).HasColumnName("comment");

            // Since we're using the Username as a foreign key, 
            // the configuration should be in line with the others.
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.Username)
                .OnDelete(DeleteBehavior.Cascade); // Adjust as per your requirements

            entity.HasOne(e => e.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(e => e.PostId);
        });

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<User> Users { get; set; }
}