using CrowdfundingAPI.Domain.Enums;

namespace CrowdfundingAPI.Application.DTOs.Campaign;

public class CreateCampaignDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public decimal GoalAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public CampaignCategory Category { get; set; }
    public List<CreateRewardTierDto> RewardTiers { get; set; } = new();
}