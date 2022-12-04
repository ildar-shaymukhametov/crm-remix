using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static CRM.Application.Constants;

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
        if (await _identityService.IsAdminAsync(_currentUserService.UserId!))
        {
            var list = await _dbContext.Companies
                .AsNoTracking()
                .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);

            foreach (var item in list)
            {
                item.CanBeEdited = true;
            }

            return list;
        }

        var requiredClaims = new[]
        {
            Claims.ViewCompany,
            Claims.DeleteCompany,
            Claims.UpdateCompany
        };

        var claims = await _identityService.GetUserAuthorizationClaimsAsync(_currentUserService.UserId!);
        if (!claims.ContainsAny(requiredClaims))
        {
            return Array.Empty<CompanyDto>();
        }

        var query = _dbContext.Companies.AsNoTracking();
        if (claims.ContainsAny(Claims.ViewCompany, Claims.DeleteCompany, Claims.UpdateCompany))
        {
            query = query.Where(x => x.ManagerId == _currentUserService.UserId);
        }

        var result = await query
            .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);

        foreach (var item in result)
        {
            item.CanBeEdited = claims.Contains(Claims.UpdateCompany);
        }

        return result;
    }
}