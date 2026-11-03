using _24_NguyenManhCuong_Assignment02_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Data
{
    public class FUNewsManagementContext : DbContext
    {
        public FUNewsManagementContext(DbContextOptions<FUNewsManagementContext> options)
            : base(options) { }

        public DbSet<SystemAccount> SystemAccounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<NewsTag> NewsTags { get; set; }
        public DbSet<NewsArticle> NewsArticles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Non-identity keys
            modelBuilder.Entity<SystemAccount>()
                .Property(a => a.AccountID)
                .ValueGeneratedNever();

            modelBuilder.Entity<Tag>()
                .Property(t => t.TagID)
                .ValueGeneratedNever();

            // Composite primary key for NewsTag
            modelBuilder.Entity<NewsTag>()
                .HasKey(nt => new { nt.NewsArticleID, nt.TagID });

            // Category self-referencing FK (restrict to avoid cascade cycle)
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // NewsArticle -> Category (cascade)
            modelBuilder.Entity<NewsArticle>()
                .HasOne(n => n.Category)
                .WithMany(c => c.NewsArticles)
                .HasForeignKey(n => n.CategoryID)
                .OnDelete(DeleteBehavior.Cascade);

            // NewsArticle -> SystemAccount (cascade)
            modelBuilder.Entity<NewsArticle>()
                .HasOne(n => n.CreatedBy)
                .WithMany(a => a.CreatedNewsArticles)
                .HasForeignKey(n => n.CreatedByID)
                .OnDelete(DeleteBehavior.Cascade);

            // NewsTag -> NewsArticle (cascade)
            modelBuilder.Entity<NewsTag>()
                .HasOne(nt => nt.NewsArticle)
                .WithMany(n => n.NewsTags)
                .HasForeignKey(nt => nt.NewsArticleID)
                .OnDelete(DeleteBehavior.Cascade);

            // NewsTag -> Tag (cascade)
            modelBuilder.Entity<NewsTag>()
                .HasOne(nt => nt.Tag)
                .WithMany(t => t.NewsTags)
                .HasForeignKey(nt => nt.TagID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
