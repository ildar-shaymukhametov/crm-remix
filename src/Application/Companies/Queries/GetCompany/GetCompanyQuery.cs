using AutoMapper;
using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Companies.Queries.GetCompany;

[Authorize(Constants.Policies.Company.Queries.View)]
public record GetCompanyQuery(int Id) : IRequest<CompanyVm>
{
}

public class GetCompanyRequestHandler : IRequestHandler<GetCompanyQuery, CompanyVm>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IAccessService _accessService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppAuthorizationService _authorizationService;

    public GetCompanyRequestHandler(IApplicationDbContext dbContext, IMapper mapper, IAccessService accessService, ICurrentUserService currentUserService, IAppAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _accessService = accessService;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<CompanyVm> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Companies
            .AsNoTracking()
            .Include(x => x.Manager)
            .Include(x => x.Type)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken) ?? throw new NotFoundException("Company", request.Id);

        var result = new CompanyVm
        {
            Id = entity.Id,
        };

        var accessRights = await _accessService.CheckAccessAsync(_currentUserService.UserId!);
        if (accessRights.ContainsAny(
            Domain.Constants.Access.Company.Any.Manager.Get,
            Domain.Constants.Access.Company.WhereUserIsManager.Manager.Get,
            Domain.Constants.Access.Company.Any.Manager.SetFromAnyToAny,
            Domain.Constants.Access.Company.Any.Manager.SetFromAnyToNone,
            Domain.Constants.Access.Company.Any.Manager.SetFromAnyToSelf,
            Domain.Constants.Access.Company.Any.Manager.SetFromSelfToAny,
            Domain.Constants.Access.Company.Any.Manager.SetFromSelfToNone,
            Domain.Constants.Access.Company.Any.Manager.SetFromNoneToAny,
            Domain.Constants.Access.Company.Any.Manager.SetFromNoneToSelf,
            Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny,
            Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone
        ))
        {
            result.Fields.Add(nameof(Company.Manager), _mapper.Map<ManagerDto>(entity.Manager));
        }

        if (accessRights.ContainsAny(
            Domain.Constants.Access.Company.Any.Other.Get,
            Domain.Constants.Access.Company.Any.Other.Set,
            Domain.Constants.Access.Company.WhereUserIsManager.Other.Get,
            Domain.Constants.Access.Company.WhereUserIsManager.Other.Set
        ))
        {
            result.Fields.Add(nameof(Company.Address), entity.Address);
            result.Fields.Add(nameof(Company.Ceo), entity.Ceo);
            result.Fields.Add(nameof(Company.Contacts), entity.Contacts);
            result.Fields.Add(nameof(Company.Email), entity.Email);
            result.Fields.Add(nameof(Company.Inn), entity.Inn);
            result.Fields.Add(nameof(Company.Phone), entity.Phone);
            result.Fields.Add(nameof(Company.Type), _mapper.Map<CompanyTypeDto>(entity.Type));
        }

        if (accessRights.ContainsAny(
            Domain.Constants.Access.Company.Any.Name.Get,
            Domain.Constants.Access.Company.Any.Name.Set,
            Domain.Constants.Access.Company.WhereUserIsManager.Name.Get,
            Domain.Constants.Access.Company.WhereUserIsManager.Name.Set
        ))
        {
            result.Fields.Add(nameof(Company.Name), entity.Name);
        }

        result.CanBeDeleted = await _authorizationService.AuthorizeAsync(_currentUserService.UserId!, entity, Constants.Policies.Company.Queries.Delete);
        result.CanBeUpdated = await _authorizationService.AuthorizeAsync(_currentUserService.UserId!, entity, Constants.Policies.Company.Queries.Update);

        return result;
    }
}