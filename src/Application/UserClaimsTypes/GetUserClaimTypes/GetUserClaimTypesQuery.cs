using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Companies.Queries.GetUserClaimsTypes;

public record GetUserClaimTypesQuery : IRequest<UserClaimTypeVm[]>;

public class GetUserClaimsRequestHandler : IRequestHandler<GetUserClaimTypesQuery, UserClaimTypeVm[]>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetUserClaimsRequestHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<UserClaimTypeVm[]> Handle(GetUserClaimTypesQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.UserClaimTypes
            .AsNoTracking()
            .ProjectTo<UserClaimTypeVm>(_mapper.ConfigurationProvider)
            .ToArrayAsync();
    }
}

public class UserClaimTypeVm : IMapFrom<UserClaimType>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Value { get; set; }
}
