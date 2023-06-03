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
    private readonly IAccessService _accessService;
    private readonly ICurrentUserService _currentUserService;

    public GetTestCompanyRequestHandler(IApplicationDbContext dbContext, IMapper mapper, IAccessService accessService, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _accessService = accessService;
        _currentUserService = currentUserService;
    }

    public async Task<CompanyVm> Handle(GetTestCompanyQuery request, CancellationToken cancellationToken)
    {
        var handler = new GetCompanyRequestHandler(_dbContext, _mapper, _accessService, _currentUserService);
        return await handler.Handle(request, cancellationToken);
    }
}