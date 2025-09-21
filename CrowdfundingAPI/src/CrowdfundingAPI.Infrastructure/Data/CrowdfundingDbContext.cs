using CrowdfundingAPI.Domain.Entities;
using CrowdfundingAPI.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CrowdfundingAPI.Infrastructure.Data;

public class CrowdfundingDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public CrowdfundingDbContext(DbContextOptions<CrowdfundingDbContext> options) : base(options)
    {
    }

    public DbSet<User> AppUsers { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Pledge> Pledges { get; set; }
    public DbSet<RewardTier> RewardTiers { get; set; }
    public DbSet<CampaignUpdate> CampaignUpdates { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // User Configuration
        builder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Bio).HasMaxLength(1000);
            entity.Property(e => e.ProfileImageUrl).HasMaxLength(500);
        });

        // Campaign Configuration
        builder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.ShortDescription).IsRequired().HasMaxLength(500);
            entity.Property(e => e.GoalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CurrentAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CoverImageUrl).HasMaxLength(500);
            entity.Property(e => e.VideoUrl).HasMaxLength(500);

            entity.HasOne(e => e.Owner)
                  .WithMany(e => e.OwnedCampaigns)
                  .HasForeignKey(e => e.OwnerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Pledge Configuration
        builder.Entity<Pledge>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PaymentIntentId).HasMaxLength(100);

            entity.HasOne(e => e.User)
                  .WithMany(e => e.Pledges)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Campaign)
                  .WithMany(e => e.Pledges)
                  .HasForeignKey(e => e.CampaignId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.RewardTier)
                  .WithMany(e => e.Pledges)
                  .HasForeignKey(e => e.RewardTierId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // RewardTier Configuration
        builder.Entity<RewardTier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.MinimumAmount).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Campaign)
                  .WithMany(e => e.RewardTiers)
                  .HasForeignKey(e => e.CampaignId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // CampaignUpdate Configuration
        builder.Entity<CampaignUpdate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).IsRequired();

            entity.HasOne(e => e.Campaign)
                  .WithMany(e => e.Updates)
                  .HasForeignKey(e => e.CampaignId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        SeedData(builder);
    }

    private void SeedData(ModelBuilder builder)
    {
        var adminUserId = Guid.NewGuid();
        var campaignOwnerId = Guid.NewGuid();
        var regularUserId = Guid.NewGuid();

        builder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@crowdfunding.com",
                Bio = "Platform Administrator",
                Role = Domain.Enums.UserRole.Admin,
                IsEmailVerified = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = campaignOwnerId,
                FirstName = "John",
                LastName = "Creator",
                Email = "creator@example.com",
                Bio = "Innovative product creator",
                Role = Domain.Enums.UserRole.CampaignOwner,
                IsEmailVerified = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = regularUserId,
                FirstName = "Jane",
                LastName = "Backer",
                Email = "backer@example.com",
                Bio = "Enthusiastic supporter of creative projects",
                Role = Domain.Enums.UserRole.User,
                IsEmailVerified = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        var campaignId = Guid.NewGuid();
        builder.Entity<Campaign>().HasData(
            new Campaign
            {
                Id = campaignId,
                Title = "Revolutionary Smart Watch",
                Description = "A comprehensive description of our innovative smart watch with health monitoring capabilities.",
                ShortDescription = "Next-generation smart watch with advanced health features",
                GoalAmount = 50000,
                CurrentAmount = 15000,
                StartDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                CoverImageUrl = "https://example.com/smartwatch.jpg",
                Status = Domain.Enums.CampaignStatus.Active,
                Category = Domain.Enums.CampaignCategory.Technology,
                OwnerId = campaignOwnerId,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}