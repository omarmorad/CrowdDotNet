using CrowdfundingAPI.Domain.Enums;

namespace CrowdfundingAPI.Domain.Entities;

public class Pledge
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CampaignId { get; set; }
    public decimal Amount { get; set; }
    public Guid? RewardTierId { get; set; }
    public PledgeStatus Status { get; set; }
    public string PaymentIntentId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Campaign Campaign { get; set; } = null!;
    public RewardTier? RewardTier { get; set; }
}