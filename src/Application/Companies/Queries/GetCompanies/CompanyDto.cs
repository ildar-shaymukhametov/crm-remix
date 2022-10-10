using CRM.App.Application.Common.Mappings;
using CRM.App.Domain.Entities;

namespace CRM.App.Application.Companies.Queries.GetCompanies;

public class CompanyDto : IMapFrom<Company>
{
    public int Id { get; set; }
    public string? Type { get; set; }
    public string? Name { get; set; }
    public string? Inn { get; set; }
    public string? Address { get; set; }
    public string? Ceo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Contacts { get; set; }
}