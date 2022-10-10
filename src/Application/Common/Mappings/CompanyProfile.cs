using AutoMapper;
using CRM.App.Application.Companies.Commands.CreateCompany;
using CRM.App.Domain.Entities;

namespace CRM.App.Application.Common.Mappings;

public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<CreateCompanyCommand, Company>(MemberList.Source);
    }
}