using CrowdfundingAPI.Application.Common;
using CrowdfundingAPI.Application.DTOs.Pledge;
using MediatR;

namespace CrowdfundingAPI.Application.Commands.Pledge;

public class CreatePledgeCommand : IRequest<ApiResponse<PledgeDto>>
{
    public Guid CampaignId { get; set; }
    public decimal Amount { get; set; }
    public Guid? RewardTierId { get; set; }
    public Guid UserId { get; set; }
}