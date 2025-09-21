using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrowdfundingAPI.API.Controllers;

/// <summary>
/// Administrative endpoints for platform management
/// </summary>
[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class AdminController : ControllerBase
{
    /// <summary>
    /// Get campaigns pending approval
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Paginated list of pending campaigns</returns>
    [HttpGet("campaigns/pending")]
    public async Task<IActionResult> GetPendingCampaigns(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        // Implementation would retrieve campaigns with UnderReview status
        return Ok(new { message = "Pending campaigns retrieved successfully" });
    }

    /// <summary>
    /// Approve a campaign
    /// </summary>
    /// <param name="id">Campaign ID</param>
    /// <returns>Success message</returns>
    [HttpPut("campaigns/{id}/approve")]
    public async Task<IActionResult> ApproveCampaign(Guid id)
    {
        // Implementation would change campaign status to Active
        return Ok(new { message = "Campaign approved successfully", id });
    }

    /// <summary>
    /// Reject a campaign
    /// </summary>
    /// <param name="id">Campaign ID</param>
    /// <param name="rejectionDto">Rejection reason</param>
    /// <returns>Success message</returns>
    [HttpPut("campaigns/{id}/reject")]
    public async Task<IActionResult> RejectCampaign(Guid id, [FromBody] CampaignRejectionDto rejectionDto)
    {
        // Implementation would change campaign status to Cancelled and notify owner
        return Ok(new { message = "Campaign rejected successfully", id });
    }

    /// <summary>
    /// Get all users with pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="searchTerm">Search term for user name or email</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        // Implementation would retrieve users with filtering
        return Ok(new { message = "Users retrieved successfully" });
    }

    /// <summary>
    /// Ban a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="banDto">Ban details</param>
    /// <returns>Success message</returns>
    [HttpPut("users/{id}/ban")]
    public async Task<IActionResult> BanUser(Guid id, [FromBody] UserBanDto banDto)
    {
        // Implementation would disable user account and cancel their campaigns
        return Ok(new { message = "User banned successfully", id });
    }

    /// <summary>
    /// Get platform analytics overview
    /// </summary>
    /// <returns>Platform statistics and metrics</returns>
    [HttpGet("analytics/overview")]
    public async Task<IActionResult> GetPlatformAnalytics()
    {
        // Implementation would return platform-wide statistics
        var analytics = new
        {
            TotalCampaigns = 150,
            ActiveCampaigns = 45,
            TotalUsers = 1250,
            TotalFundsRaised = 2500000.00m,
            SuccessfulCampaigns = 89,
            AverageSuccessRate = 59.3m,
            TopCategories = new[]
            {
                new { Category = "Technology", Count = 35 },
                new { Category = "Arts", Count = 28 },
                new { Category = "Games", Count = 22 }
            }
        };

        return Ok(new { success = true, data = analytics });
    }
}

public class CampaignRejectionDto
{
    public string Reason { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
}

public class UserBanDto
{
    public string Reason { get; set; } = string.Empty;
    public DateTime? BanUntil { get; set; }
    public bool IsPermanent { get; set; }
}