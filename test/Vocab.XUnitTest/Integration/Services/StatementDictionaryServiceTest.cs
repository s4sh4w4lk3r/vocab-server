using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vocab.Application.Abstractions.Services;
using Vocab.Core.Entities;
using Vocab.Infrastructure.Persistence;
using Vocab.Infrastructure.Services;
using Vocab.XUnitTest.Shared;

#pragma warning disable IDE0052 // Remove unread private members
namespace Vocab.XUnitTest.Integration.Services
{
    [TestCaseOrderer(
    ordererTypeName: "Vocab.XUnitTest.Shared.PriorityOrderer",
    ordererAssemblyName: PriorityOrderer.ASSMEBLY_NAME)]
    public class StatementDictionaryServiceTest
    {
        private static readonly ServiceProvider serviceProvider;

        private readonly long validPKey = 1;
        private readonly Guid validOwnerFKey = Guid.Parse("53929fa6-5b34-47f9-8d97-6a4dc46f2067");

        private static bool test1Called = false;
        private static bool test2Called = false;
        private static bool test3Called = false;
        private static bool test4Called = false;
        static StatementDictionaryServiceTest()
        {
            ServiceCollection services = new();
            ConfigurationBuilder builder = new();
            builder.AddUserSecrets("5b793eb0-da62-410a-be80-5f2a05718376");
            IConfigurationRoot configuration = builder.Build();

            services.AddDbContext<VocabContext>(options => options.UseNpgsql(configuration.GetConnectionString("Postgres")));
            services.AddScoped<IStatementDictionaryService, StatementDictionaryService>();

            serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VocabContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        [Fact, TestPriority(1)]
        public async Task Insert()
        {
            test1Called = true;
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VocabContext>();
            var service = scope.ServiceProvider.GetRequiredService<IStatementDictionaryService>();

            var result = await service.Insert(new StatementDictionary(0, "DictTest1", validOwnerFKey, DateTime.UtcNow));

            Assert.True(result.Success);
            Assert.NotEqual(0, result.Value?.Id);
            Assert.True(db.StatementDictionaries.Any(sd => sd.Id == result.Value!.Id && result.Value.OwnerId == validOwnerFKey));
        }

        [Theory, TestPriority(2)]
        [InlineData(false, "")]
        [InlineData(false, " ")]
        [InlineData(true, "DictTest2")]
        public async Task SetName(bool expected, string name)
        {
            test2Called = true;
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VocabContext>();
            var service = scope.ServiceProvider.GetRequiredService<IStatementDictionaryService>();

            var result = await service.SetName(validOwnerFKey, validPKey, name);

            Assert.Equal(expected, result.Success);
            Assert.Equal(expected, db.StatementDictionaries.Any(sd=>sd.Id == validPKey && sd.OwnerId == validOwnerFKey && sd.Name == name));
        }

        [Theory, TestPriority(3)]
        [InlineData(false, "")]
        [InlineData(false, " ")]
        [InlineData(true, "DictTest3")]
        public async Task Update(bool expected, string name)
        {
            test3Called = true;
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VocabContext>();
            var service = scope.ServiceProvider.GetRequiredService<IStatementDictionaryService>();
            var dict = new StatementDictionary(validPKey, name, validOwnerFKey, DateTime.UtcNow);

            var result = await service.Update(dict);

            Assert.Equal(expected, result.Success);
            Assert.Equal(expected, db.StatementDictionaries.Any(sd => sd.Id == validPKey && sd.OwnerId == validOwnerFKey && sd.Name == name));
        }

        [Theory, TestPriority(4)]
        [InlineData(false, "53929fa6-5b34-47f9-8d97-6a4dc46f2067", 0)]
        [InlineData(false, "00000000-0000-0000-0000-000000000000", 1)]
        [InlineData(true, "53929fa6-5b34-47f9-8d97-6a4dc46f2067", 1)]
        public async Task Delete(bool expected, string userIdStr, long dictionaryId)
        {
            test4Called = true;
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VocabContext>();
            var service = scope.ServiceProvider.GetRequiredService<IStatementDictionaryService>();

            var result = await service.Delete(Guid.Parse(userIdStr), dictionaryId);

            Assert.Equal(expected, result.Success);
            Assert.Equal(expected, !db.StatementDictionaries.Any(sd => sd.Id == validPKey));
        }
    }
}


#pragma warning restore IDE0052 // Remove unread private members