using CRM.Application.Common.Mappings;
using CRM.Domain.Entities;

namespace CRM.Application.Companies.Queries.GetCompany;

public class CompanyVm : IMapFrom<Company>
{
    public int Id { get; set; }
    public Dictionary<string, object?> Fields { get; set; } = new();
    public bool CanBeDeleted { get; set; }
}

public class ManagerDto : IMapFrom<ApplicationUser>
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class CompanyTypeDto : IMapFrom<CompanyType>
{
    public int Id { get; set; }
    public string? Name { get; set; }
}