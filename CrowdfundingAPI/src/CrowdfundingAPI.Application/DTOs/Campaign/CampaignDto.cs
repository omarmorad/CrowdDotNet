using CrowdfundingAPI.Domain.Enums;

namespace CrowdfundingAPI.Application.DTOs.Campaign;

public class CampaignDto
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
    public decimal FundingPercentage { get; set; }
    public bool IsActive { get; set; }
    public bool IsFunded { get; set; }
    
    public UserDto Owner { get; set; } = null!;
    public List<RewardTierDto> RewardTiers { get; set; } = new();
    public int TotalBackers { get; set; }
}