using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Repository;

namespace Scheduler.Repository;

internal class MatrixRepository : IMatrixRepository
{
    private readonly openapiClient _client;

    public MatrixRepository(IOptions<MatrixRepositoryOptions> options, HttpClient client)
    {
        _client = new openapiClient(options.Value.Host, client);
    }

    /// <summary>
    /// Gets a retry policy for the service.
    /// </summary>
    /// <param name="provider">
    /// The service provider.
    /// </param>
    /// <returns>
    /// The retry policy.
    /// </returns>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IServiceProvider provider)
    {
        var logger = provider.GetRequiredService<ILogger<MatrixRepository>>();
        var delay = Backoff.DecorrelatedJitterBackoffV2(
            medianFirstRetryDelay: TimeSpan.FromSeconds(1),
            retryCount: 5
        );
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                delay,
                onRetry: (r, d) =>
                    logger.LogWarning(
                        r.Exception,
                        "Failed to execute request. Waiting {Delay}ms before retry",
                        d
                    )
            );
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(int rows, int columns, string hash, CancellationToken token)
    {
        var result = await _client.CreateMatrixAsync(
            new MatrixCreationOptions
            {
                Rows = rows,
                Columns = columns,
                Hash = hash
            },
            token
        );

        return result.MatrixId;
    }

    /// <inheritdoc />
    public async Task<bool> IsComputed(int id, CancellationToken token)
    {
        var result = await _client.IsMatrixComputedAsync(id, token);
        return result.IsComputed;
    }
}
