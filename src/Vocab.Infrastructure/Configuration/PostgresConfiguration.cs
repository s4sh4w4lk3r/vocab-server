namespace Vocab.Infrastructure.Configuration
{
    public class PostgresConfiguration
    {
        public required string ConnectionString { get; init; }
        public string? Collation { get; init; }
        public bool SensitiveDataLoggingEnabled { get; init; }
    }
}
