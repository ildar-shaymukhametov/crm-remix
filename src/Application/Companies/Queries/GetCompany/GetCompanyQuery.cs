using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Companies.Queries.GetCompany;

[Authorize(Constants.Policies.GetCompany)]
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
        var item = await _dbContext.Companies
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .ProjectTo<CompanyVm>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        return item ?? throw new NotFoundException("Company", request.Id);
    }
}