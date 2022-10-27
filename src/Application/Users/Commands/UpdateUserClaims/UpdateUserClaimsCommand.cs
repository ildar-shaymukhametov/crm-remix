using AutoMapper;
using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using MediatR;

namespace CRM.Application.Users.Commands.UpdateUserClaims;

public record UpdateUserClaimsCommand : IRequest<Unit>
{
    public string[] Claims { get; set; } = Array.Empty<string>();
}

public class UpdateUserClaimsCommandHandler : IRequestHandler<UpdateUserClaimsCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;

    public UpdateUserClaimsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper, IIdentityService identityService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _identityService = identityService;
    }

    public async Task<Unit> Handle(UpdateUserClaimsCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId == null)
        {
            throw new UnauthorizedAccessException();
        }

        var result = await _identityService.UpdateClaimsAsync(_currentUserService.UserId, request.Claims);
        if (result.Errors.Any())
        {
            throw new InternalServerErrorException($"Failed to update claims for user {_currentUserService.UserId}");
        }

        return Unit.Value;
    }
}
