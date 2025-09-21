using CrowdfundingAPI.Domain.Enums;

namespace CrowdfundingAPI.Domain.Entities;

public class Campaign
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public decimal GoalAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public CampaignStatus Status { get; set; }
    public CampaignCategory Category { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User Owner { get; set; } = null!;
    public List<Pledge> Pledges { get; set; } = new();
    public List<RewardTier> RewardTiers { get; set; } = new();
    public List<CampaignUpdate> Updates { get; set; } = new();

    // Calculated properties
    public decimal FundingPercentage => GoalAmount > 0 ? (CurrentAmount / GoalAmount) * 100 : 0;
    public bool IsActive => Status == CampaignStatus.Active && DateTime.UtcNow <= EndDate;
    public bool IsFunded => CurrentAmount >= GoalAmount;
}