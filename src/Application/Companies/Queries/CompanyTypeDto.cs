using CRM.Application.Common.Mappings;
using CRM.Domain.Entities;

namespace CRM.Application.Companies.Queries;

public class CompanyTypeDto : IMapFrom<CompanyType>
{
    public int Id { get; set; }
    public string? Name { get; set; }
}