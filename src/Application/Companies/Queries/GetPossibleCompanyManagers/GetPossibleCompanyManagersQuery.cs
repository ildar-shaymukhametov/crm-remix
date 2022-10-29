using AutoMapper;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Companies.Queries.GetPossibleCompanyManagers;

public record GetPossibleCompanyManagersQuery : IRequest<PossibleCompanyManagersVm>
{
    public int Id { get; set; }
}

public class GetCompanyRequestHandler : IRequestHandler<GetPossibleCompanyManagersQuery, PossibleCompanyManagersVm>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetCompanyRequestHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<PossibleCompanyManagersVm> Handle(GetPossibleCompanyManagersQuery request, CancellationToken cancellationToken)
    {
        var items = await _dbContext.ApplicationUsers
            .AsNoTracking()
            .ProjectToListAsync<ManagerDto>(_mapper.ConfigurationProvider);

        return new PossibleCompanyManagersVm
        {
            Managers = items
        };
    }
}