using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Riton.Application.Todos.Commands.InsertTodosFromCsvs;
using Riton.Application.Todos.Queries.Inquery;

namespace Riton.Web.Endpoints;

public class Todos : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .MapGet(Inquery)
            .MapPost(InsertTodosFromCsv);
    }

    public async Task<InqueryStateDto> Inquery(ISender sender, string idempotencyKey)
    {

        return await sender.Send(new InqueryQuery { IdempotencyId= idempotencyKey });
    }
    public async Task<IResult> InsertTodosFromCsv(ISender sender, IFormFile csv, [FromHeader] string idempotencyKey, CancellationToken cancellationToken)
    {
        if (HasInValidIputs(csv, idempotencyKey)) return Results.BadRequest();


        var filePath = Path.Combine("App_Data", $"{idempotencyKey}.csv");
        try
        {
            using var stream = new FileStream(filePath, FileMode.Create);
            await csv.CopyToAsync(stream, cancellationToken);
            var cmd = new InsertTodosFromCsvCommand { FilePath = filePath, IdempotencyKey = idempotencyKey };
            BackgroundJob.Enqueue<ISender>(sender => sender.Send(cmd, cancellationToken));
            return Results.NoContent();
        }
        catch
        {
            return Results.BadRequest();
        }
    }

    private static bool HasInValidIputs(IFormFile csv, string idempotencyKey)
    {
        bool isInValid = false;
        if (csv == null || csv.Length == 0 || csv.ContentType is not "text/csv" || !Path.GetExtension(csv.FileName).Equals(".csv", StringComparison.CurrentCultureIgnoreCase))
            isInValid = true;

        if (string.IsNullOrWhiteSpace(idempotencyKey) || idempotencyKey.Length < 3)
        {
            isInValid = true;
        }
        return isInValid;
    }
}
