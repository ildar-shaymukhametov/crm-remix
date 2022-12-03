using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Companies.Queries.GetCompanies;

[Authorize]
public record GetCompaniesQuery : IRequest<CompanyDto[]>;

public class GetCompaniesRequestHandler : IRequestHandler<GetCompaniesQuery, CompanyDto[]>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public GetCompaniesRequestHandler(IApplicationDbContext dbContext, IMapper mapper, ICurrentUserService currentUserService, IIdentityService identityService)
    {
        _currentUserService = currentUserService;
        _identityService = identityService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CompanyDto[]> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {

        var claims = await _identityService.GetUserAuthorizationClaimsAsync(_currentUserService.UserId!);
        if (claims.Contains(Constants.Claims.ViewCompany) || claims.Contains(Constants.Claims.DeleteCompany) || claims.Contains(Constants.Claims.UpdateCompany))
        {
            return await _dbContext.Companies
                .AsNoTracking()
                .Where(x => x.ManagerId == _currentUserService.UserId)
                .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);
        }

        return Array.Empty<CompanyDto>();
    }
}