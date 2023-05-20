using AutoMapper;
using CRM.Application.Common.Interfaces;
using MediatR;

namespace CRM.Application.Companies.Queries.GetCompany;

public record GetTestCompanyQuery : GetCompanyQuery
{
}

public class GetTestCompanyRequestHandler : IRequestHandler<GetTestCompanyQuery, CompanyVm>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetTestCompanyRequestHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CompanyVm> Handle(GetTestCompanyQuery request, CancellationToken cancellationToken)
    {
        var handler = new GetCompanyRequestHandler(_dbContext, _mapper);
        return await handler.Handle(request, cancellationToken);
    }
}