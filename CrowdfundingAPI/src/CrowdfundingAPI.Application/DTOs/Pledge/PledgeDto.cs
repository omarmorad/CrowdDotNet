using CrowdfundingAPI.Application.DTOs.Campaign;
using CrowdfundingAPI.Domain.Enums;

namespace CrowdfundingAPI.Application.DTOs.Pledge;

public class PledgeDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CampaignId { get; set; }
    public decimal Amount { get; set; }
    public Guid? RewardTierId { get; set; }
    public PledgeStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    
    public UserDto User { get; set; } = null!;
    public CampaignDto Campaign { get; set; } = null!;
    public RewardTierDto? RewardTier { get; set; }
}