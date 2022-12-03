using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Interfaces;
using CRM.Application.Companies.Commands.DeleteCompany;
using CRM.Application.Companies.Commands.UpdateCompany;
using CRM.Application.Companies.Queries.GetCompany;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Common.Behaviours.Authorization;

public interface IResourceProvider
{
    Task<object?> GetResourceAsync<TRequest>(string policy, TRequest request);
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

    public async Task<object?> GetResourceAsync<TRequest>(string policy, TRequest request)
    {
        return policy switch
        {
            Constants.Policies.GetCompany when request is GetCompanyQuery query => await GetCompanyAsync(query.Id),
            Constants.Policies.UpdateCompany when request is UpdateCompanyCommand query => await GetCompanyAsync(query.Id),
            Constants.Policies.DeleteCompany when request is DeleteCompanyCommand query => await GetCompanyAsync(query.Id),
            _ => null,
        };
    }

    private async Task<CompanyDto?> GetCompanyAsync(int id)
    {
        return await _dbContext.Companies
            .Where(x => x.Id == id)
            .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }
}
