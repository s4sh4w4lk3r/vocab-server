using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vocab.Application.Abstractions.Services;
using Vocab.Infrastructure.Persistence;
using Vocab.Infrastructure.Services;
using Vocab.WebApi.Controllers;
using Vocab.XUnitTest.Shared;

namespace Vocab.XUnitTest.Integration
{
    [TestCaseOrderer(
    ordererTypeName: "Vocab.XUnitTest.Shared.PriorityOrderer",
    ordererAssemblyName: Constants.ASSEMBLY_NAME)]
    public class DictionaryControllerTest
    {
        private static readonly StatementDictionaryController controller;
        static DictionaryControllerTest()
        {
            ServiceProvider serviceProvider;
            ServiceCollection services = new();
            ConfigurationBuilder builder = new();
            builder.AddUserSecrets(Constants.USER_SECRET_ID);
            IConfigurationRoot configuration = builder.Build();

            services.AddDbContext<VocabContext>(options => options.UseNpgsql(configuration.GetConnectionString("Postgres")));
            services.AddScoped<IStatementDictionaryService, StatementDictionaryService>();

            serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VocabContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            controller = new(scope.ServiceProvider.GetRequiredService<IStatementDictionaryService>());
        }
    }
}
