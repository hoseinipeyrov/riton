namespace Riton.Domain.Entities;

public class TodoItem : BaseAuditableEntity
{
    public string? Title { get; set; }

    public string? Note { get; set; }

    public int TodoId { get; set; }
    public Todo Todo { get; set; } = null!;
}
