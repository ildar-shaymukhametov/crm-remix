using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Entities;
using MediatR;

namespace CRM.Application.Companies.Commands.UpdateCompany;

[Authorize(Constants.Policies.Company.Commands.Update)]
public record UpdateCompanyCommand(int Id) : IRequest
{
    public Dictionary<string, object?> Fields { get; set; } = new();
}

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IAccessService _accessService;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCompanyCommandHandler(IApplicationDbContext context, IAccessService accessService, ICurrentUserService currentUserService)
    {
        _context = context;
        _accessService = accessService;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Companies.FindAsync(new object?[] { request.Id }, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException("Company", request.Id);
        }

        var accessRights = await _accessService.CheckAccessAsync(_currentUserService.UserId!);
        if (accessRights.ContainsAny(
            Constants.Access.Company.Any.Manager.SetFromAnyToAny,
            Constants.Access.Company.Any.Manager.SetFromAnyToNone,
            Constants.Access.Company.Any.Manager.SetFromAnyToSelf,
            Constants.Access.Company.Any.Manager.SetFromNoneToAny,
            Constants.Access.Company.Any.Manager.SetFromNoneToSelf,
            Constants.Access.Company.Any.Manager.SetFromSelfToAny,
            Constants.Access.Company.Any.Manager.SetFromSelfToNone,
            Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny,
            Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone
        ))
        {
            entity.ManagerId = (string?)request.Fields[nameof(Company.ManagerId)];
        }

        if (accessRights.ContainsAny(
            Constants.Access.Company.WhereUserIsManager.Other.Set,
            Constants.Access.Company.Any.Other.Set
        ))
        {
            entity.Address = (string?)request.Fields[nameof(Company.Address)];
            entity.Ceo = (string?)request.Fields[nameof(Company.Ceo)];
            entity.Contacts = (string?)request.Fields[nameof(Company.Contacts)];
            entity.Email = (string?)request.Fields[nameof(Company.Email)];
            entity.Inn = (string?)request.Fields[nameof(Company.Inn)];
            entity.Phone = (string?)request.Fields[nameof(Company.Phone)];
            entity.TypeId = (int?)request.Fields[nameof(Company.TypeId)];
        }

        if (accessRights.ContainsAny(
            Constants.Access.Company.WhereUserIsManager.Name.Set,
            Constants.Access.Company.Any.Name.Set
        ))
        {
            entity.Name = (string)request.Fields[nameof(Company.Name)]!;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return;
    }
}
