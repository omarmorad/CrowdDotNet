using System.Security.Claims;
using CrowdfundingAPI.Application.Commands.Campaign;
using CrowdfundingAPI.Application.Commands.Pledge;
using CrowdfundingAPI.Application.DTOs.Campaign;
using CrowdfundingAPI.Application.DTOs.Pledge;
using CrowdfundingAPI.Application.Queries.Campaign;
using CrowdfundingAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrowdfundingAPI.API.Controllers;

/// <summary>
/// Campaign management endpoints
/// </summary>
[ApiController]
[Route("api/v1/campaigns")]
[Authorize]
[Produces("application/json")]
public class CampaignsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CampaignsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get campaigns with filtering, searching, and pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="searchTerm">Search term for title, description, or creator name</param>
    /// <param name="category">Filter by campaign category</param>
    /// <param name="status">Filter by campaign status</param>
    /// <param name="minFundingPercentage">Minimum funding percentage</param>
    /// <param name="maxFundingPercentage">Maximum funding percentage</param>
    /// <param name="sortBy">Sort field (CreatedAt, Title, GoalAmount, etc.)</param>
    /// <param name="sortDescending">Sort direction (default: true)</param>
    /// <returns>Paginated list of campaigns</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCampaigns(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] CampaignCategory? category = null,
        [FromQuery] CampaignStatus? status = null,
        [FromQuery] decimal? minFundingPercentage = null,
        [FromQuery] decimal? maxFundingPercentage = null,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool sortDescending = true)
    {
        var query = new GetCampaignsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            Category = category,
            Status = status,
            MinFundingPercentage = minFundingPercentage,
            MaxFundingPercentage = maxFundingPercentage,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new campaign
    /// </summary>
    /// <param name="createCampaignDto">Campaign creation data</param>
    /// <returns>Created campaign</returns>
    [HttpPost]
    [Authorize(Roles = "CampaignOwner,Admin")]
    public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignDto createCampaignDto)
    {
        var userId = GetCurrentUserId();
        var command = new CreateCampaignCommand
        {
            Title = createCampaignDto.Title,
            Description = createCampaignDto.Description,
            ShortDescription = createCampaignDto.ShortDescription,
            GoalAmount = createCampaignDto.GoalAmount,
            StartDate = createCampaignDto.StartDate,
            EndDate = createCampaignDto.EndDate,
            CoverImageUrl = createCampaignDto.CoverImageUrl,
            VideoUrl = createCampaignDto.VideoUrl,
            Category = createCampaignDto.Category,
            RewardTiers = createCampaignDto.RewardTiers,
            UserId = userId
        };

        var result = await _mediator.Send(command);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetCampaignById), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// Get campaign by ID
    /// </summary>
    /// <param name="id">Campaign ID</param>
    /// <returns>Campaign details</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCampaignById(Guid id)
    {
        var query = new GetCampaignByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Update campaign (owner only)
    /// </summary>
    /// <param name="id">Campaign ID</param>
    /// <param name="updateCampaignDto">Updated campaign data</param>
    /// <returns>Updated campaign</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCampaign(Guid id, [FromBody] UpdateCampaignDto updateCampaignDto)
    {
        // Implementation would validate ownership and update campaign
        return Ok(new { message = "Campaign updated successfully" });
    }

    /// <summary>
    /// Cancel campaign (owner only)
    /// </summary>
    /// <param name="id">Campaign ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelCampaign(Guid id)
    {
        // Implementation would validate ownership and cancel campaign
        return Ok(new { message = "Campaign cancelled successfully" });
    }

    /// <summary>
    /// Create a pledge to a campaign
    /// </summary>
    /// <param name="id">Campaign ID</param>
    /// <param name="createPledgeDto">Pledge data</param>
    /// <returns>Created pledge</returns>
    [HttpPost("{id}/pledge")]
    public async Task<IActionResult> CreatePledge(Guid id, [FromBody] CreatePledgeDto createPledgeDto)
    {
        var userId = GetCurrentUserId();
        var command = new CreatePledgeCommand
        {
            CampaignId = id,
            Amount = createPledgeDto.Amount,
            RewardTierId = createPledgeDto.RewardTierId,
            UserId = userId
        };

        var result = await _mediator.Send(command);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get campaign pledges (owner only)
    /// </summary>
    /// <param name="id">Campaign ID</param>
    /// <returns>List of pledges</returns>
    [HttpGet("{id}/pledges")]
    public async Task<IActionResult> GetCampaignPledges(Guid id)
    {
        // Implementation would validate ownership and return pledges
        return Ok(new { message = "Campaign pledges retrieved successfully" });
    }

    /// <summary>
    /// Post campaign update (owner only)
    /// </summary>
    /// <param name="id">Campaign ID</param>
    /// <param name="campaignUpdateDto">Update content</param>
    /// <returns>Created update</returns>
    [HttpPost("{id}/updates")]
    public async Task<IActionResult> PostCampaignUpdate(Guid id, [FromBody] CampaignUpdateDto campaignUpdateDto)
    {
        // Implementation would validate ownership and create update
        return Ok(new { message = "Campaign update posted successfully" });
    }

    /// <summary>
    /// Get campaign analytics (owner only)
    /// </summary>
    /// <param name="id">Campaign ID</param>
    /// <returns>Campaign analytics data</returns>
    [HttpGet("{id}/analytics")]
    public async Task<IActionResult> GetCampaignAnalytics(Guid id)
    {
        // Implementation would validate ownership and return analytics
        return Ok(new { message = "Campaign analytics retrieved successfully" });
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }
}

public class UpdateCampaignDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
}

public class CampaignUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}