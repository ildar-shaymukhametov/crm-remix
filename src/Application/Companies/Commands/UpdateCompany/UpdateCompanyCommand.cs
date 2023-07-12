using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Companies.Commands.UpdateCompany;

[Authorize(Constants.Policies.Company.Commands.Update)]
public record UpdateCompanyCommand(int Id) : IRequest
{
    public int? TypeId { get; set; }
    public string Name { get; set; } = default!;
    public string? Inn { get; set; }
    public string? Address { get; set; }
    public string? Ceo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Contacts { get; set; }
    public string? ManagerId { get; set; }

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
            entity.ManagerId = request.ManagerId;
        }

        if (accessRights.ContainsAny(
            Constants.Access.Company.WhereUserIsManager.Other.Set,
            Constants.Access.Company.Any.Other.Set
        ))
        {
            entity.Address = request.Address;
            entity.Ceo = request.Ceo;
            entity.Contacts = request.Contacts;
            entity.Email = request.Email;
            entity.Inn = request.Inn;
            entity.Phone = request.Phone;
            entity.TypeId = request.TypeId;

        }

        if (accessRights.ContainsAny(
            Constants.Access.Company.WhereUserIsManager.Name.Set,
            Constants.Access.Company.Any.Name.Set
        ))
        {
            entity.Name = request.Name;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return;
    }
}
