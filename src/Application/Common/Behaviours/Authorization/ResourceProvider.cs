using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Common.Behaviours.Authorization;

public interface IResourceProvider
{
    Task<CompanyDto?> GetCompanyAsync(int id);
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
}
