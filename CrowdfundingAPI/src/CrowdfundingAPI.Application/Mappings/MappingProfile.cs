using AutoMapper;
using CrowdfundingAPI.Application.DTOs;
using CrowdfundingAPI.Application.DTOs.Campaign;
using CrowdfundingAPI.Application.DTOs.Pledge;
using CrowdfundingAPI.Domain.Entities;

namespace CrowdfundingAPI.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        
        CreateMap<Campaign, CampaignDto>()
            .ForMember(dest => dest.TotalBackers, opt => opt.MapFrom(src => src.Pledges.Count(p => p.Status == Domain.Enums.PledgeStatus.Confirmed)));
        
        CreateMap<RewardTier, RewardTierDto>();
        
        CreateMap<Pledge, PledgeDto>();
        
        CreateMap<CreateCampaignDto, Campaign>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.CampaignStatus.Draft))
            .ForMember(dest => dest.CurrentAmount, opt => opt.MapFrom(src => 0));
        
        CreateMap<CreateRewardTierDto, RewardTier>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CurrentBackers, opt => opt.MapFrom(src => 0));
    }
}