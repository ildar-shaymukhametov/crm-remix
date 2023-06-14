using CRM.Application.Common.Mappings;
using CRM.Domain.Entities;

namespace CRM.Application.Companies.Queries;

public class CompanyVm : IMapFrom<Company>
{
    public int Id { get; set; }
    public Dictionary<string, object?> Fields { get; set; } = new();
    public bool CanBeDeleted { get; set; }
    public bool CanBeUpdated { get; set; }
}
