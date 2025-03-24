namespace Persistence.Configuration;

public class DatabaseConfig
{
    public const string SectionName = "Database";
    public bool UseInMemory { get; set; }
    public string InMemoryDatabaseName { get; set; } = null!;
    public bool MigrationEnabled { get; set; }
    public bool SeedEnabled { get; set; }
    public int CommandTimeout { get; set; }
    public int? MaxRetryCount { get; set; }
}
