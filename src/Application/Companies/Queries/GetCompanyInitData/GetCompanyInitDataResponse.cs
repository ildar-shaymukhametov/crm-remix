using CRM.Application.Common.Mappings;
using CRM.Domain.Entities;

namespace CRM.Application.Companies.Queries.GetCompanyManagers;

public record GetCompanyInitDataResponse
{
    public List<ManagerDto> Managers { get; set; } = new List<ManagerDto>();
    public List<CompanyTypeDto> CompanyTypes { get; internal set; } = new List<CompanyTypeDto>();
}

public record ManagerDto : IMapFrom<ApplicationUser>
{
    public string Id { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public record CompanyTypeDto : IMapFrom<CompanyType>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}
