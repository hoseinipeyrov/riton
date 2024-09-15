
using Riton.Application.Common.Interfaces;

namespace Riton.Application.Todos.Queries.Inquery;
public record InqueryQuery : IRequest<InqueryStateDto>
{
    public required string IdempotencyId { get; set; }
}

public class InqueryQueryHandler(IApplicationDbContext context) : IRequestHandler<InqueryQuery, InqueryStateDto>
{
    public async Task<InqueryStateDto> Handle(InqueryQuery request, CancellationToken cancellationToken)
    {
        var todo = await context.Todos.FirstOrDefaultAsync(t => t.Title == request.IdempotencyId, cancellationToken: cancellationToken);
        if (todo is {})
        {
            return new InqueryStateDto
            {
                IdempotencyId = request.IdempotencyId,
                State = "Completed"
            };
        }

        // not complete
        var filePath = Path.Combine("App_Data", $"{request.IdempotencyId}.csv");
        if (File.Exists(filePath))
            return new InqueryStateDto
            {
                IdempotencyId = request.IdempotencyId,
                State = "InProgress"
            };

        return new InqueryStateDto
        {
            IdempotencyId = request.IdempotencyId,
            State = "Not Upladed"
        };
    }
}
