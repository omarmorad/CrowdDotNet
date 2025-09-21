using CrowdfundingAPI.Application.Common;
using CrowdfundingAPI.Application.DTOs.Campaign;
using CrowdfundingAPI.Domain.Enums;
using MediatR;

namespace CrowdfundingAPI.Application.Queries.Campaign;

public class GetCampaignsQuery : IRequest<ApiResponse<PagedResult<CampaignDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public CampaignCategory? Category { get; set; }
    public CampaignStatus? Status { get; set; }
    public decimal? MinFundingPercentage { get; set; }
    public decimal? MaxFundingPercentage { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}