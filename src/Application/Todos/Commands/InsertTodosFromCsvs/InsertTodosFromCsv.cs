using System.Globalization;
using CsvHelper;
using Riton.Application.Common.Interfaces;
using Riton.Domain.Entities;

namespace Riton.Application.Todos.Commands.InsertTodosFromCsvs;

public record InsertTodosFromCsvCommand : IRequest
{
    public required string FilePath { get; init; }
    public required string IdempotencyKey { get; set; }
}

public class InsertTodosFromCsvCommandHandler(IApplicationDbContext context) : IRequestHandler<InsertTodosFromCsvCommand>
{
    public async Task Handle(InsertTodosFromCsvCommand request, CancellationToken cancellationToken)
    {
        var todos = new Todo
        {
            Title = request.IdempotencyKey
        };
        using (var reader = new StreamReader(request.FilePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<dynamic>();
            foreach (var record in records)
            {
                todos.Items.Add(new TodoItem
                {
                    Title = record.Title,
                    Note = record.Note
                });
            }
        }

        context.Todos.Add(todos);
        await context.SaveChangesAsync(cancellationToken);
    }
}
