using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Mappings;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Common.Behaviours.Authorization;

public interface IResourceProvider
{
    Task<CompanyDto?> GetCompanyAsync(int id);
    Task<List<CompanyDto>> GetCompaniesAsync(params int[] ids);
}

public class ResourceProvider : IResourceProvider
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ResourceProvider(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CompanyDto?> GetCompanyAsync(int id)
    {
        return await _dbContext.Companies
            .Where(x => x.Id == id)
            .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<List<CompanyDto>> GetCompaniesAsync(params int[] ids)
    {
        return await _dbContext.Companies
            .Where(x => ids.Contains(x.Id))
            .ProjectToListAsync<CompanyDto>(_mapper.ConfigurationProvider);
    }
}
