namespace CrowdfundingAPI.Application.DTOs.Pledge;

public class CreatePledgeDto
{
    public decimal Amount { get; set; }
    public Guid? RewardTierId { get; set; }
}