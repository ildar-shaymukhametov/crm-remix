using CRM.Application.Common.Mappings;
using CRM.Domain.Entities;

namespace CRM.Application.Companies.Queries.GetCompanyManagers;

public record GetCompanyManagersResponse
{
    public List<ManagerDto> Managers { get; set; } = new List<ManagerDto>();
}

public record ManagerDto : IMapFrom<ApplicationUser>
{
    public string Id { get; set; } = default!;
}
