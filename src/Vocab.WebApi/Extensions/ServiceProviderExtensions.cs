using Microsoft.EntityFrameworkCore;
using Throw;

namespace Vocab.WebApi.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static async Task EnsureDatabaseCanConnect<TDbContext>(this IServiceProvider serviceProvider) where TDbContext : DbContext
        {
            serviceProvider.ThrowIfNull();

            TDbContext db = serviceProvider.GetRequiredService<TDbContext>();
            bool canConnect = await db.Database.CanConnectAsync();
            //canConnect.Throw(_ => new InvalidOperationException("Не получилось подключиться к базе данных.")).IfFalse();
            //await db.Database.EnsureCreatedAsync();
        }

    }
}
