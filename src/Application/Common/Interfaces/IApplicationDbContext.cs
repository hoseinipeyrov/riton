using Riton.Domain.Entities;

namespace Riton.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Todo> Todos { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
