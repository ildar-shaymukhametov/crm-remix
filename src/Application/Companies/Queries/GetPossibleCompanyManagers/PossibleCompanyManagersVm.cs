using CRM.Application.Common.Mappings;
using CRM.Domain.Entities;

namespace CRM.Application.Companies.Queries.GetPossibleCompanyManagers;

public class PossibleCompanyManagersVm
{
    public List<ManagerDto>? Managers { get; set; }
}

public class ManagerDto : IMapFrom<ApplicationUser>
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}