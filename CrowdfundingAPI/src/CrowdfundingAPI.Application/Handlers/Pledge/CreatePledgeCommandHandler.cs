using AutoMapper;
using CrowdfundingAPI.Application.Commands.Pledge;
using CrowdfundingAPI.Application.Common;
using CrowdfundingAPI.Application.DTOs.Pledge;
using CrowdfundingAPI.Application.Interfaces;
using CrowdfundingAPI.Domain.Entities;
using CrowdfundingAPI.Domain.Enums;
using CrowdfundingAPI.Domain.Interfaces;
using MediatR;

namespace CrowdfundingAPI.Application.Handlers.Pledge;

public class CreatePledgeCommandHandler : IRequestHandler<CreatePledgeCommand, ApiResponse<PledgeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;
    private readonly IMapper _mapper;

    public CreatePledgeCommandHandler(
        IUnitOfWork unitOfWork,
        IPaymentService paymentService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PledgeDto>> Handle(CreatePledgeCommand request, CancellationToken cancellationToken)
    {
        // Validate campaign exists and is active
        var campaign = await _unitOfWork.Campaigns.GetByIdAsync(request.CampaignId);
        if (campaign == null)
        {
            return ApiResponse<PledgeDto>.ErrorResult("Campaign not found");
        }

        if (!campaign.IsActive)
        {
            return ApiResponse<PledgeDto>.ErrorResult("Campaign is not active");
        }

        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return ApiResponse<PledgeDto>.ErrorResult("User not found");
        }

        // Validate reward tier if specified
        RewardTier? rewardTier = null;
        if (request.RewardTierId.HasValue)
        {
            rewardTier = await _unitOfWork.RewardTiers.GetByIdAsync(request.RewardTierId.Value);
            if (rewardTier == null || rewardTier.CampaignId != request.CampaignId)
            {
                return ApiResponse<PledgeDto>.ErrorResult("Invalid reward tier");
            }

            if (!rewardTier.IsAvailable)
            {
                return ApiResponse<PledgeDto>.ErrorResult("Reward tier is not available");
            }

            if (request.Amount < rewardTier.MinimumAmount)
            {
                return ApiResponse<PledgeDto>.ErrorResult($"Minimum pledge amount for this reward tier is {rewardTier.MinimumAmount:C}");
            }
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Process payment
            var paymentResult = await _paymentService.ProcessPaymentAsync(request.Amount, "credit_card");
            
            var pledge = new Domain.Entities.Pledge
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                CampaignId = request.CampaignId,
                Amount = request.Amount,
                RewardTierId = request.RewardTierId,
                Status = paymentResult.IsSuccess ? PledgeStatus.Confirmed : PledgeStatus.Failed,
                PaymentIntentId = paymentResult.TransactionId,
                CreatedAt = DateTime.UtcNow,
                ProcessedAt = paymentResult.IsSuccess ? DateTime.UtcNow : null
            };

            await _unitOfWork.Pledges.AddAsync(pledge);

            if (paymentResult.IsSuccess)
            {
                // Update campaign current amount
                campaign.CurrentAmount += request.Amount;
                campaign.UpdatedAt = DateTime.UtcNow;

                // Check if campaign is now funded
                if (campaign.IsFunded && campaign.Status == CampaignStatus.Active)
                {
                    campaign.Status = CampaignStatus.Funded;
                }

                _unitOfWork.Campaigns.Update(campaign);

                // Update reward tier backer count
                if (rewardTier != null)
                {
                    rewardTier.CurrentBackers++;
                    _unitOfWork.RewardTiers.Update(rewardTier);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // Load pledge with related data
            pledge.User = user;
            pledge.Campaign = campaign;
            pledge.RewardTier = rewardTier;

            var pledgeDto = _mapper.Map<PledgeDto>(pledge);
            
            var message = paymentResult.IsSuccess 
                ? "Pledge created successfully" 
                : $"Pledge failed: {paymentResult.Message}";

            return ApiResponse<PledgeDto>.SuccessResult(pledgeDto, message);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ApiResponse<PledgeDto>.ErrorResult("An error occurred while processing the pledge");
        }
    }
}