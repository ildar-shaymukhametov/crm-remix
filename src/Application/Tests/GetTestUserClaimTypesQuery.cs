using AutoMapper;
using CRM.Application.Common.Interfaces;
using MediatR;

namespace CRM.Application.Companies.Queries.GetUserClaimsTypes;

public record GetTestUserClaimTypesQuery : GetUserClaimTypesQuery;

public class GetTestUserClaimsRequestHandler : IRequestHandler<GetTestUserClaimTypesQuery, UserClaimTypeVm[]>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetTestUserClaimsRequestHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<UserClaimTypeVm[]> Handle(GetTestUserClaimTypesQuery request, CancellationToken cancellationToken)
    {
        var handler = new GetUserClaimsRequestHandler(_dbContext, _mapper);
        return await handler.Handle(request, cancellationToken);
    }
}
