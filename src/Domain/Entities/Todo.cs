namespace Riton.Domain.Entities;

public class Todo : BaseAuditableEntity
{
    public required string Title { get; set; }

    public ICollection<TodoItem> Items { get; set; } = [];
}
