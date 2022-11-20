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
    private readonly IPermissionsService _permissionsService;
    private readonly ICurrentUserService _currentUserService;

    public GetCompaniesRequestHandler(IApplicationDbContext dbContext, IMapper mapper, IPermissionsService permissionsService, ICurrentUserService currentUserService)
    {
        _permissionsService = permissionsService;
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CompanyDto[]> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _permissionsService.CheckUserPermissionsAsync(_currentUserService.UserId!, new[] { "ViewCompany" });

        if (permissions.Contains("ViewCompany"))
        {
            return await _dbContext.Companies
                .AsNoTracking()
                .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);
        }

        return Array.Empty<CompanyDto>();
    }
}