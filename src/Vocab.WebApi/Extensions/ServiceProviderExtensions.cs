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
            canConnect.Throw(_ => new InvalidOperationException("Не получилось подключиться к базе данных.")).IfFalse();
            //await db.Database.EnsureCreatedAsync();
        }

        public static async Task EnsureKeycloakCanConnect(this IServiceProvider serviceProvider, Uri kcUri)
        {
            serviceProvider.ThrowIfNull();
            kcUri.ThrowIfNull();
            string kcExceptionMessage = "Не получилось подключиться к серверу аутентификации.";

            using HttpClient httpClient = new();

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(kcUri);
                response.IsSuccessStatusCode.Throw(_ => new InvalidOperationException($"{kcExceptionMessage} Статусный код: {(int)response.StatusCode}")).IfFalse();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(kcExceptionMessage, ex);
            }
        }
    }
}
