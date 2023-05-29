using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Repository;

namespace Scheduler;

internal class MatrixRepository : IMatrixRepository
{
    private readonly ILogger _logger;
    private readonly openapiClient _clinet;

    public MatrixRepository(
        ILogger<MatrixRepository> logger,
        IOptions<MatrixRepositoryOptions> options,
        HttpClient client
    )
    {
        _logger = logger;
        _clinet = new openapiClient(options.Value.Host, client);
    }

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
                onRetry: (result, delay) =>
                    logger.LogWarning(
                        result.Exception,
                        "Failed to execute request. Waiting {Delay}ms before retry",
                        delay
                    )
            );
    }

    public async Task<int> CreateAsync(int rows, int columns, string hash, CancellationToken token)
    {
        var result = await _clinet.CreateMatrixAsync(
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

    public async Task<bool> IsComputed(int id, CancellationToken token)
    {
        var result = await _clinet.IsMatrixComputedAsync(id, token);
        return result.IsComputed;
    }
}
