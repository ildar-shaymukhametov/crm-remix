using AutoMapper;
using CRM.Application.Common.Interfaces;
using MediatR;

namespace CRM.Application.Companies.Commands.UpdateClaims;

public record UpdateClaimsCommand : IRequest<Unit>
{
    public string[] Claims { get; set; } = Array.Empty<string>();
}

public class UpdateClaimsCommandHandler : IRequestHandler<UpdateClaimsCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;

    public UpdateClaimsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper, IIdentityService identityService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _identityService = identityService;
    }

    public async Task<Unit> Handle(UpdateClaimsCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId == null)
        {
            throw new UnauthorizedAccessException();
        }

        var result = await _identityService.UpdateClaimsAsync(_currentUserService.UserId, request.Claims);
        if (result.Errors.Any())
        {
            throw new InvalidOperationException($"Failed to update claims for user {_currentUserService.UserId}");
        }

        return Unit.Value;
    }
}
