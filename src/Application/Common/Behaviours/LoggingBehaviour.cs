using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Riton.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest>(ILogger<TRequest> logger) : IRequestPreProcessor<TRequest> where TRequest : notnull
{

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        await Task.Run(() => logger.LogInformation("Riton Request: {Name}  {@Request}", requestName, request), cancellationToken);
    }
}
