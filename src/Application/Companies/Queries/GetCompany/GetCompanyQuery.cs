using AutoMapper;
using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Companies.Queries.GetCompany;

[Authorize(Constants.Policies.Company.View)]
public record GetCompanyQuery(int id) : IRequest<CompanyVm>
{
    public int Id => id;
}

public class GetCompanyRequestHandler : IRequestHandler<GetCompanyQuery, CompanyVm>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IAccessService _accessService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public GetCompanyRequestHandler(IApplicationDbContext dbContext, IMapper mapper, IAccessService accessService, ICurrentUserService currentUserService, IIdentityService identityService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _accessService = accessService;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<CompanyVm> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Companies
            .AsNoTracking()
            .Include(x => x.Manager)
            .Include(x => x.Type)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException("Company", request.Id);
        }

        var result = new CompanyVm
        {
            Id = entity.Id
        };

        var accessRights = await _accessService.CheckAccessAsync(_currentUserService.UserId!);
        if (accessRights.ContainsAny(Constants.Access.Company.Any.Manager.View, Constants.Access.Company.WhereUserIsManager.Manager.View, Constants.Access.Company.Any.Manager.SetFromSelfToAny, Constants.Access.Company.Any.Manager.SetFromSelfToNone))
        {
            result.Fields.Add(nameof(Company.Manager), _mapper.Map<ManagerDto>(entity.Manager));
        }

        if (accessRights.ContainsAny(Constants.Access.Company.Any.Other.View, Constants.Claims.Company.Any.Other.Update, Constants.Access.Company.WhereUserIsManager.Other.View))
        {
            result.Fields.Add(nameof(Company.Address), entity.Address);
            result.Fields.Add(nameof(Company.Ceo), entity.Ceo);
            result.Fields.Add(nameof(Company.Contacts), entity.Contacts);
            result.Fields.Add(nameof(Company.Email), entity.Email);
            result.Fields.Add(nameof(Company.Inn), entity.Inn);
            result.Fields.Add(nameof(Company.Name), entity.Name);
            result.Fields.Add(nameof(Company.Phone), entity.Phone);
            result.Fields.Add(nameof(Company.Type), _mapper.Map<CompanyTypeDto>(entity.Type));
        }

        result.CanBeDeleted = await _identityService.AuthorizeAsync(_currentUserService.UserId!, entity, Constants.Policies.Company.Delete);
        result.CanBeUpdated = await _identityService.AuthorizeAsync(_currentUserService.UserId!, entity, Constants.Policies.Company.Update);

        return result;
    }
}