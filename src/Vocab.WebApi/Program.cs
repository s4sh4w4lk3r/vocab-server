using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Serilog;
using Throw;
using Vocab.Application.Configuration;
using Vocab.Infrastructure.Persistence;

namespace Vocab.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console()
            .WriteTo.File($"./logs/log.log", rollingInterval: RollingInterval.Day)
            .ReadFrom.Configuration(ctx.Configuration));

            ConfigureServices(builder);
            ConfigureOptions(builder);

            // -------------------------------------------------------------------------- >8

            var app = builder.Build();
            ConfigureMiddlewares(app);

            app.MapControllers();

            Test(app);


            app.Run();
        }
        private static void ConfigureOptions(WebApplicationBuilder builder)
        {
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });

            builder.Services.Configure<PostgresConfiguration>(builder.Configuration.GetRequiredSection(nameof(PostgresConfiguration)));
            builder.Services.Configure<CorsConfiguration>(builder.Configuration.GetRequiredSection(nameof(CorsConfiguration)));
        }
        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddDbContext<VocabDbContext>(contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Scoped);
        }
        private static void ConfigureMiddlewares(WebApplication app)
        {
            app.UseForwardedHeaders();

            /*app.UseAuthentication();
            app.UseAuthorization();*/

            var origins = app.Services.GetRequiredService<IOptions<CorsConfiguration>>().Value.Origins;
            app.UseCors(o => o.AllowAnyMethod().AllowAnyHeader().WithOrigins(origins));
        }
        private static void Test(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
            var db = scope.ServiceProvider.GetRequiredService<VocabDbContext>();


            db.Database.CanConnect().Throw(_ => new InvalidOperationException("Не получилось подключиться к базе данных.")).IfFalse();
            // TODO: добавить проверку кейклока через http.
        }
    }
}
// https://github.com/edinSahbaz/clean-api-template?tab=readme-ov-file