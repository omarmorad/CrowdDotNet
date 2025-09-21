using CrowdfundingAPI.Domain.Enums;

namespace CrowdfundingAPI.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public List<Campaign> OwnedCampaigns { get; set; } = new();
    public List<Pledge> Pledges { get; set; } = new();
}