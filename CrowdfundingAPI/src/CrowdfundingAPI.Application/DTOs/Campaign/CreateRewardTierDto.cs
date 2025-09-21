namespace CrowdfundingAPI.Application.DTOs.Campaign;

public class CreateRewardTierDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MinimumAmount { get; set; }
    public int? MaxBackers { get; set; }
    public DateTime? EstimatedDelivery { get; set; }
}