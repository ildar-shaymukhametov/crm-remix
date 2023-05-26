using CRM.Application.Common.Interfaces;
using MediatR;

namespace CRM.Application.Tests;

public record ResetDbCommand : IRequest { }

public class ResetDbCommandHandler : IRequestHandler<ResetDbCommand>
{
    private readonly ITestService _testService;

    public ResetDbCommandHandler(ITestService testService)
    {
        _testService = testService;
    }

    public async Task Handle(ResetDbCommand request, CancellationToken cancellationToken)
    {
        await _testService.ResetDbAsync();
        return;
    }
}
