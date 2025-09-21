using AutoMapper;
using CrowdfundingAPI.Application.Common;
using CrowdfundingAPI.Application.DTOs.Campaign;
using CrowdfundingAPI.Application.Queries.Campaign;
using CrowdfundingAPI.Domain.Interfaces;
using MediatR;

namespace CrowdfundingAPI.Application.Handlers.Campaign;

public class GetCampaignsQueryHandler : IRequestHandler<GetCampaignsQuery, ApiResponse<PagedResult<CampaignDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCampaignsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<CampaignDto>>> Handle(GetCampaignsQuery request, CancellationToken cancellationToken)
    {
        var campaigns = await _unitOfWork.Campaigns.GetAllAsync();
        
        // Apply filters
        var filteredCampaigns = campaigns.AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            filteredCampaigns = filteredCampaigns.Where(c => 
                c.Title.ToLower().Contains(searchTerm) ||
                c.Description.ToLower().Contains(searchTerm) ||
                c.Owner.FirstName.ToLower().Contains(searchTerm) ||
                c.Owner.LastName.ToLower().Contains(searchTerm));
        }

        if (request.Category.HasValue)
        {
            filteredCampaigns = filteredCampaigns.Where(c => c.Category == request.Category.Value);
        }

        if (request.Status.HasValue)
        {
            filteredCampaigns = filteredCampaigns.Where(c => c.Status == request.Status.Value);
        }

        if (request.MinFundingPercentage.HasValue)
        {
            filteredCampaigns = filteredCampaigns.Where(c => c.FundingPercentage >= request.MinFundingPercentage.Value);
        }

        if (request.MaxFundingPercentage.HasValue)
        {
            filteredCampaigns = filteredCampaigns.Where(c => c.FundingPercentage <= request.MaxFundingPercentage.Value);
        }

        // Apply sorting
        filteredCampaigns = request.SortBy?.ToLower() switch
        {
            "title" => request.SortDescending ? filteredCampaigns.OrderByDescending(c => c.Title) : filteredCampaigns.OrderBy(c => c.Title),
            "goalamount" => request.SortDescending ? filteredCampaigns.OrderByDescending(c => c.GoalAmount) : filteredCampaigns.OrderBy(c => c.GoalAmount),
            "currentamount" => request.SortDescending ? filteredCampaigns.OrderByDescending(c => c.CurrentAmount) : filteredCampaigns.OrderBy(c => c.CurrentAmount),
            "enddate" => request.SortDescending ? filteredCampaigns.OrderByDescending(c => c.EndDate) : filteredCampaigns.OrderBy(c => c.EndDate),
            "fundingpercentage" => request.SortDescending ? filteredCampaigns.OrderByDescending(c => c.FundingPercentage) : filteredCampaigns.OrderBy(c => c.FundingPercentage),
            _ => request.SortDescending ? filteredCampaigns.OrderByDescending(c => c.CreatedAt) : filteredCampaigns.OrderBy(c => c.CreatedAt)
        };

        var totalCount = filteredCampaigns.Count();
        var pagedCampaigns = filteredCampaigns
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var campaignDtos = _mapper.Map<List<CampaignDto>>(pagedCampaigns);

        var result = new PagedResult<CampaignDto>
        {
            Items = campaignDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return ApiResponse<PagedResult<CampaignDto>>.SuccessResult(result);
    }
}