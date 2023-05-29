using System.Diagnostics;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Scheduler;

internal record ScheduleResult(int MatrixId);

internal static class JobController
{
    public static async Task<IResult> ScheduleAsync(
        [FromServices] IMatrixRepository repository,
        [FromServices] Distributor distributor,
        [FromServices] IValidator<IFormFileCollection> validator,
        IFormFileCollection files
    )
    {
        var validationResult = await validator.ValidateAsync(files);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var first = files["First"];
        var second = files["Second"];

        Debug.Assert(first != null);
        Debug.Assert(second != null);

        using var firstStream = first.OpenReadStream();
        using var secondStream = second.OpenReadStream();

        var firstMatrixStream = new MatrixStreamReader(firstStream);
        var secondMatrixStream = new MatrixStreamReader(secondStream);

        if (firstMatrixStream.Columns != secondMatrixStream.Rows)
        {
            return Results.BadRequest("Wrong matrix sizes");
        }

        var hash = $"{firstMatrixStream.ComputeHash()}{secondMatrixStream.ComputeHash()}";
        var id = await repository.CreateAsync(
            firstMatrixStream.Rows,
            secondMatrixStream.Columns,
            hash,
            default
        );

        var result = new ScheduleResult(id);

        var computed = await repository.IsComputed(id, default);
        if (computed)
        {
            return Results.Ok(result);
        }

        await distributor.ScheduleAsync(id, firstMatrixStream, secondMatrixStream);
        return Results.Ok(result);
    }
}
