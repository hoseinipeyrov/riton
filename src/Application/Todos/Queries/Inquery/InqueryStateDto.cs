namespace Riton.Application.Todos.Queries.Inquery;

public class InqueryStateDto
{
    public required string IdempotencyId { get; set; }
    public required string State { get; set; }
}
