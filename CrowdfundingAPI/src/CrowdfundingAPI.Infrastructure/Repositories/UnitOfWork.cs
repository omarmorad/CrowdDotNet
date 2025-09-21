using CrowdfundingAPI.Domain.Entities;
using CrowdfundingAPI.Domain.Interfaces;
using CrowdfundingAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace CrowdfundingAPI.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly CrowdfundingDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(CrowdfundingDbContext context)
    {
        _context = context;
        Users = new Repository<User>(_context);
        Campaigns = new Repository<Campaign>(_context);
        Pledges = new Repository<Pledge>(_context);
        RewardTiers = new Repository<RewardTier>(_context);
        CampaignUpdates = new Repository<CampaignUpdate>(_context);
    }

    public IRepository<User> Users { get; }
    public IRepository<Campaign> Campaigns { get; }
    public IRepository<Pledge> Pledges { get; }
    public IRepository<RewardTier> RewardTiers { get; }
    public IRepository<CampaignUpdate> CampaignUpdates { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}