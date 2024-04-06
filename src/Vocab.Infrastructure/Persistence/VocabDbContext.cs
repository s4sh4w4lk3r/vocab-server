using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vocab.Application.Configuration;

namespace Vocab.Infrastructure.Persistence
{
    public class VocabDbContext(IOptions<PostgresConfiguration> options, ILoggerFactory loggerFactory) : DbContext
    {
        private readonly PostgresConfiguration _configuration = options.Value ?? throw new ArgumentNullException(nameof(options), "Параметры подключения к БД не получены.");
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(loggerFactory);
            optionsBuilder.EnableSensitiveDataLogging(sensitiveDataLoggingEnabled: _configuration.SensitiveDataLoggingEnabled);
            optionsBuilder.UseNpgsql(_configuration.ConnectionString, options => options.MigrationsAssembly("Vocab.WebApi"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
