namespace CrowdfundingAPI.Domain.Entities;

public class RewardTier
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MinimumAmount { get; set; }
    public int? MaxBackers { get; set; }
    public int CurrentBackers { get; set; }
    public DateTime? EstimatedDelivery { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public Campaign Campaign { get; set; } = null!;
    public List<Pledge> Pledges { get; set; } = new();

    // Calculated properties
    public bool IsAvailable => IsActive && (MaxBackers == null || CurrentBackers < MaxBackers);
}