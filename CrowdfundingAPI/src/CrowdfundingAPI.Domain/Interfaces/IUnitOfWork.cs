using CrowdfundingAPI.Domain.Entities;

namespace CrowdfundingAPI.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Campaign> Campaigns { get; }
    IRepository<Pledge> Pledges { get; }
    IRepository<RewardTier> RewardTiers { get; }
    IRepository<CampaignUpdate> CampaignUpdates { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}