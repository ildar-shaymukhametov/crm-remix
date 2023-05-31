using CRM.Application.Common.Mappings;
using CRM.Domain.Entities;

namespace CRM.Application.Common.Behaviours.Authorization.Resources;

public class CompanyDto : IMapFrom<Company>
{
    public int Id { get; set; }
    public string? ManagerId { get; set; }
    public string? Address { get; set; }
    public string? Ceo { get; set; }
    public string? Contacts { get; set; }
    public string? Email { get; set; }
    public string? Inn { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public int? TypeId { get; set; }
}
