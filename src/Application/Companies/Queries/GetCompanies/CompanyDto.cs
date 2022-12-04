using CRM.Application.Common.Mappings;
using CRM.Domain.Entities;

namespace CRM.Application.Companies.Queries.GetCompanies;

public class CompanyDto : IMapFrom<Company>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool CanBeEdited { get; set; }
    public bool CanBeDeleted { get; set; }
}