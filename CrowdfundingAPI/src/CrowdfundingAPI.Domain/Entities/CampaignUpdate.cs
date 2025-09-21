namespace CrowdfundingAPI.Domain.Entities;

public class CampaignUpdate
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Campaign Campaign { get; set; } = null!;
}