using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vocab.Infrastructure.Persistence;

namespace Vocab.Test
{
    public class UnitTest1
    {
        private readonly ServiceProvider serviceProvider;

        public UnitTest1()
        {
            ServiceCollection services = new();
            services.AddDbContext<VocabContext>(options =>
            options.UseInMemoryDatabase("Test"));

            serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void Test1()
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VocabContext>();
            Assert.NotNull(db);
        }
    }
}