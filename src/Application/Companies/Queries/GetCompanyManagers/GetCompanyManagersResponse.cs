using CRM.Application.Common.Mappings;
using CRM.Domain.Entities;

namespace CRM.Application.Companies.Queries.GetCompanyManagers;

public record GetCompanyManagersResponse
{
    public ManagerDto[] Managers { get; set; } = Array.Empty<ManagerDto>();
}

public record ManagerDto : IMapFrom<ApplicationUser>
{
    public string Id { get; set; } = default!;
}
