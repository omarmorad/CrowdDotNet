using AutoMapper;
using CrowdfundingAPI.Application.Commands.Campaign;
using CrowdfundingAPI.Application.Common;
using CrowdfundingAPI.Application.DTOs.Campaign;
using CrowdfundingAPI.Domain.Entities;
using CrowdfundingAPI.Domain.Enums;
using CrowdfundingAPI.Domain.Interfaces;
using MediatR;

namespace CrowdfundingAPI.Application.Handlers.Campaign;

public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, ApiResponse<CampaignDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCampaignCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CampaignDto>> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
    {
        // Validate user exists and has appropriate role
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return ApiResponse<CampaignDto>.ErrorResult("User not found");
        }

        if (user.Role == UserRole.User)
        {
            // Upgrade user to CampaignOwner
            user.Role = UserRole.CampaignOwner;
            _unitOfWork.Users.Update(user);
        }

        // Create campaign
        var campaign = new Domain.Entities.Campaign
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            ShortDescription = request.ShortDescription,
            GoalAmount = request.GoalAmount,
            CurrentAmount = 0,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CoverImageUrl = request.CoverImageUrl,
            VideoUrl = request.VideoUrl,
            Category = request.Category,
            Status = CampaignStatus.Draft,
            OwnerId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Campaigns.AddAsync(campaign);

        // Create reward tiers
        var rewardTiers = request.RewardTiers.Select(rt => new RewardTier
        {
            Id = Guid.NewGuid(),
            CampaignId = campaign.Id,
            Title = rt.Title,
            Description = rt.Description,
            MinimumAmount = rt.MinimumAmount,
            MaxBackers = rt.MaxBackers,
            CurrentBackers = 0,
            EstimatedDelivery = rt.EstimatedDelivery,
            IsActive = true
        }).ToList();

        await _unitOfWork.RewardTiers.AddRangeAsync(rewardTiers);
        await _unitOfWork.SaveChangesAsync();

        // Load campaign with related data for response
        var createdCampaign = await _unitOfWork.Campaigns.FirstOrDefaultAsync(c => c.Id == campaign.Id);
        if (createdCampaign != null)
        {
            createdCampaign.Owner = user;
            createdCampaign.RewardTiers = rewardTiers;
        }

        var campaignDto = _mapper.Map<CampaignDto>(createdCampaign);
        return ApiResponse<CampaignDto>.SuccessResult(campaignDto, "Campaign created successfully");
    }
}