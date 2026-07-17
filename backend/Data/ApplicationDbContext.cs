using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AuctionItem> AuctionItems => Set<AuctionItem>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Bid> Bids => Set<Bid>();
        public DbSet<WishlistEntry> WishlistEntries => Set<WishlistEntry>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<CategoryItem> Categories => Set<CategoryItem>();
        public DbSet<ForumPost> ForumPosts => Set<ForumPost>();
        public DbSet<ForumComment> ForumComments => Set<ForumComment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>() // Relatie Review -> Reviewer
                .HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>() // Relatie Review -> ReviewedUser
                .HasOne(r => r.ReviewedUser)
                .WithMany(u => u.ReviewList)
                .HasForeignKey(r => r.ReviewedUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AuctionItem>() // Relatie Item -> Categorie
                .HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AuctionItem>() // Relatie Item -> Owner
                .HasOne(i => i.Owner)
                .WithMany(o => o.AddedItemsList)
                .HasForeignKey(i => i.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AuctionItem>() // Relatie Item -> Winner
                .HasOne(i => i.Winner)
                .WithMany(w => w.WonItemsList)
                .HasForeignKey(i => i.WinnerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Item)
                .WithMany(i => i.Bids)
                .HasForeignKey(b => b.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Bidder)
                .WithMany(u => u.Bids)
                .HasForeignKey(b => b.BidderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<WishlistEntry>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishlist)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WishlistEntry>()
                .HasOne(w => w.Item)
                .WithMany()
                .HasForeignKey(w => w.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WishlistEntry>()
                .HasIndex(w => new { w.UserId, w.ItemId })
                .IsUnique();

            modelBuilder.Entity<ForumPost>()
                .HasOne(p => p.Author)
                .WithMany()
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ForumComment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ForumComment>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
