using Microsoft.AspNetCore.Identity;

namespace CrowdfundingAPI.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid AppUserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; }
}