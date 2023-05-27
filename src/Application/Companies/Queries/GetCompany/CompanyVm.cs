using CRM.Application.Common.Mappings;
using CRM.Domain.Entities;

namespace CRM.Application.Companies.Queries.GetCompany;

public class CompanyVm : IMapFrom<Company>
{
    public int Id { get; set; }
    public CompanyTypeDto? Type { get; set; }
    public string? Name { get; set; }
    public string? Inn { get; set; }
    public string? Address { get; set; }
    public string? Ceo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Contacts { get; set; }
    public ManagerDto? Manager { get; set; }
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