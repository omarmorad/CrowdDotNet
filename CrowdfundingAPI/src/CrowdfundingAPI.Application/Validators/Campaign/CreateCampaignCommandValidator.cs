using CrowdfundingAPI.Application.Commands.Campaign;
using FluentValidation;

namespace CrowdfundingAPI.Application.Validators.Campaign;

public class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Campaign title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Campaign description is required")
            .MinimumLength(100).WithMessage("Description must be at least 100 characters long");

        RuleFor(x => x.ShortDescription)
            .NotEmpty().WithMessage("Short description is required")
            .MaximumLength(500).WithMessage("Short description must not exceed 500 characters");

        RuleFor(x => x.GoalAmount)
            .GreaterThan(0).WithMessage("Goal amount must be greater than 0")
            .LessThanOrEqualTo(10000000).WithMessage("Goal amount cannot exceed $10,000,000");

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Start date cannot be in the past");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date")
            .LessThanOrEqualTo(x => x.StartDate.AddDays(365)).WithMessage("Campaign duration cannot exceed 365 days");

        RuleFor(x => x.CoverImageUrl)
            .Must(BeAValidUrl).WithMessage("Cover image URL is not valid")
            .When(x => !string.IsNullOrEmpty(x.CoverImageUrl));

        RuleFor(x => x.VideoUrl)
            .Must(BeAValidUrl).WithMessage("Video URL is not valid")
            .When(x => !string.IsNullOrEmpty(x.VideoUrl));

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid campaign category");

        RuleForEach(x => x.RewardTiers)
            .SetValidator(new CreateRewardTierValidator());
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}

public class CreateRewardTierValidator : AbstractValidator<CrowdfundingAPI.Application.DTOs.Campaign.CreateRewardTierDto>
{
    public CreateRewardTierValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Reward tier title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Reward tier description is required")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.MinimumAmount)
            .GreaterThan(0).WithMessage("Minimum amount must be greater than 0");

        RuleFor(x => x.MaxBackers)
            .GreaterThan(0).WithMessage("Max backers must be greater than 0")
            .When(x => x.MaxBackers.HasValue);

        RuleFor(x => x.EstimatedDelivery)
            .GreaterThan(DateTime.UtcNow).WithMessage("Estimated delivery must be in the future")
            .When(x => x.EstimatedDelivery.HasValue);
    }
}