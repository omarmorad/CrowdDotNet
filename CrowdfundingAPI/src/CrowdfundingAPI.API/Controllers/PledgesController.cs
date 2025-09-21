using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrowdfundingAPI.API.Controllers;

/// <summary>
/// Pledge management endpoints
/// </summary>
[ApiController]
[Route("api/v1/pledges")]
[Authorize]
[Produces("application/json")]
public class PledgesController : ControllerBase
{
    /// <summary>
    /// Get current user's pledges
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Paginated list of user's pledges</returns>
    [HttpGet("my-pledges")]
    public async Task<IActionResult> GetMyPledges(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = GetCurrentUserId();
        // Implementation would retrieve user's pledges
        return Ok(new { message = "User pledges retrieved successfully", userId });
    }

    /// <summary>
    /// Get pledge by ID (owner only)
    /// </summary>
    /// <param name="id">Pledge ID</param>
    /// <returns>Pledge details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPledgeById(Guid id)
    {
        // Implementation would validate ownership and return pledge
        return Ok(new { message = "Pledge retrieved successfully", id });
    }

    /// <summary>
    /// Update pledge amount (owner only, before campaign ends)
    /// </summary>
    /// <param name="id">Pledge ID</param>
    /// <param name="updatePledgeDto">Updated pledge data</param>
    /// <returns>Updated pledge</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePledge(Guid id, [FromBody] UpdatePledgeDto updatePledgeDto)
    {
        // Implementation would validate ownership and update pledge
        return Ok(new { message = "Pledge updated successfully", id });
    }

    /// <summary>
    /// Cancel pledge (owner only, before campaign ends)
    /// </summary>
    /// <param name="id">Pledge ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelPledge(Guid id)
    {
        // Implementation would validate ownership and cancel pledge
        return Ok(new { message = "Pledge cancelled successfully", id });
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }
}

public class UpdatePledgeDto
{
    public decimal Amount { get; set; }
    public Guid? RewardTierId { get; set; }
}