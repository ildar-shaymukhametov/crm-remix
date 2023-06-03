using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Companies.Queries.GetCompany;

[Authorize(Constants.Policies.Company.View)]
public record GetCompanyQuery : IRequest<CompanyVm>
{
    public int Id { get; set; }
}

public class GetCompanyRequestHandler : IRequestHandler<GetCompanyQuery, CompanyVm>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetCompanyRequestHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CompanyVm> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Companies
            .AsNoTracking()
            .Include(x => x.Manager)
            .Include(x => x.Type)
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException("Company", request.Id);
        }

        var result = new CompanyVm
        {
            Id = entity.Id
        };

        result.Fields.Add(nameof(Company.Address), entity.Address);
        result.Fields.Add(nameof(Company.Ceo), entity.Ceo);
        result.Fields.Add(nameof(Company.Contacts), entity.Contacts);
        result.Fields.Add(nameof(Company.Email), entity.Email);
        result.Fields.Add(nameof(Company.Inn), entity.Inn);
        result.Fields.Add(nameof(Company.Manager), _mapper.Map<ManagerDto>(entity.Manager));
        result.Fields.Add(nameof(Company.Name), entity.Name);
        result.Fields.Add(nameof(Company.Phone), entity.Phone);
        result.Fields.Add(nameof(Company.Type), _mapper.Map<CompanyTypeDto>(entity.Type));

        return result;
    }
}