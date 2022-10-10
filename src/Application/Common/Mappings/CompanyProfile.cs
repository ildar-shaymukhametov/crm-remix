using AutoMapper;
using CRM.Application.Companies.Commands.CreateCompany;
using CRM.Domain.Entities;

namespace CRM.Application.Common.Mappings;

public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<CreateCompanyCommand, Company>(MemberList.Source);
    }
}