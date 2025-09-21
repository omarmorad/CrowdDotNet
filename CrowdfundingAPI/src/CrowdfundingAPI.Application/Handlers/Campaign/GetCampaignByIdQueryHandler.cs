using AutoMapper;
using CrowdfundingAPI.Application.Common;
using CrowdfundingAPI.Application.DTOs.Campaign;
using CrowdfundingAPI.Application.Queries.Campaign;
using CrowdfundingAPI.Domain.Interfaces;
using MediatR;

namespace CrowdfundingAPI.Application.Handlers.Campaign;

public class GetCampaignByIdQueryHandler : IRequestHandler<GetCampaignByIdQuery, ApiResponse<CampaignDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCampaignByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CampaignDto>> Handle(GetCampaignByIdQuery request, CancellationToken cancellationToken)
    {
        var campaign = await _unitOfWork.Campaigns.GetByIdAsync(request.Id);
        
        if (campaign == null)
        {
            return ApiResponse<CampaignDto>.ErrorResult("Campaign not found");
        }

        var campaignDto = _mapper.Map<CampaignDto>(campaign);
        return ApiResponse<CampaignDto>.SuccessResult(campaignDto);
    }
}