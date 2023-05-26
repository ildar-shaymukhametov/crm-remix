using CRM.Application.Common.Mappings;
using CRM.Domain.Entities;

namespace CRM.Application.Common.Behaviours.Authorization.Resources;

public class CompanyDto : IMapFrom<Company>
{
    public int Id { get; set; }
    public string? ManagerId { get; set; }
}
