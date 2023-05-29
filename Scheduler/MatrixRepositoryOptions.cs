using Microsoft.Extensions.Configuration;

namespace Scheduler;

internal class MatrixRepositoryOptions
{
    public const string SectionName = "MatrixRepository";
    public string Host { get; set; } = string.Empty;

    public void With(IConfiguration configuration) =>
        configuration.GetRequiredSection(SectionName).Bind(this);
}
