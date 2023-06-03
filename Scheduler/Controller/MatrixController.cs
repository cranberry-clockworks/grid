using System.Diagnostics;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Scheduler.Repository;
using Scheduler.Scheduler;

namespace Scheduler.Controller;

/// <summary>
/// An API controller to schedule matrix multiplications.
/// </summary>
internal static class MatrixController
{
    /// <summary>
    /// Schedule matrix multiplication.
    /// </summary>
    /// <param name="repository">
    /// The matrix repository service.
    /// </param>
    /// <param name="productTaskScheduler">
    /// The task scheduler.
    /// </param>
    /// <param name="validator">
    /// The validator for input parameters.
    /// </param>
    /// <param name="files">
    /// Matrix files.
    /// </param>
    public static async Task<IResult> ScheduleAsync(
        [FromServices] IMatrixRepository repository,
        [FromServices] ProductTaskScheduler productTaskScheduler,
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

        await using var firstStream = first.OpenReadStream();
        await using var secondStream = second.OpenReadStream();

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

        await productTaskScheduler.ScheduleAsync(id, firstMatrixStream, secondMatrixStream);
        return Results.Ok(result);
    }

    private record ScheduleResult(int MatrixId);
}
