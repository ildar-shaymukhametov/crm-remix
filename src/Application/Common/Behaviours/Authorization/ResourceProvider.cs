using CRM.Application.Common.Interfaces;
using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Common.Behaviours.Authorization;

public interface IResourceProvider
{
    Task<Company?> GetCompanyAsync(int id);
    Task<List<Company>> GetCompaniesAsync(params int[] ids);
}

public class ResourceProvider : IResourceProvider
{
    private readonly IApplicationDbContext _dbContext;

    public ResourceProvider(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Company?> GetCompanyAsync(int id)
    {
        return await _dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Company>> GetCompaniesAsync(params int[] ids)
    {
        return await _dbContext.Companies
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();
    }
}
