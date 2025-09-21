using CrowdfundingAPI.Application.Common;
using CrowdfundingAPI.Application.DTOs.Campaign;
using MediatR;

namespace CrowdfundingAPI.Application.Queries.Campaign;

public class GetCampaignByIdQuery : IRequest<ApiResponse<CampaignDto>>
{
    public Guid Id { get; set; }
}