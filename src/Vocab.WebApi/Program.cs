using HotChocolate.Types;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Serilog;
using Throw;
using Vocab.Infrastructure.Configuration;
using Vocab.Infrastructure.Persistence;
using Vocab.WebApi.Extensions;

namespace Vocab.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // -------------------------------------------------------------------------------------------------------------------------- >8

            builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console()
            .WriteTo.File($"./logs/log.log", rollingInterval: RollingInterval.Day)
            .ReadFrom.Configuration(ctx.Configuration));

            // -------------------------------------------------------------------------------------------------------------------------- >8

            builder.Services.AddControllers();
            builder.Services.AddDbContext<VocabContext>(contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Scoped);

            IConfigurationSection kcConfigurationSection = builder.Configuration.GetRequiredSection(nameof(KeycloakConfiguration));
            KeycloakConfiguration kcConfiguration = kcConfigurationSection.Get<KeycloakConfiguration>() ?? throw new ArgumentNullException("Конфигурация Keycloak не получена.");
            builder.Services.AddAuthenticationVocab(kcConfiguration);
            builder.Services.AddAuthorizationVocab(kcConfiguration);

            builder.Services.AddSwaggerVocab();
            builder.Services.AddGraphQLVocab(isDevelopment: builder.Environment.IsDevelopment());



            // -------------------------------------------------------------------------------------------------------------------------- >8

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });

            builder.Services.Configure<PostgresConfiguration>(builder.Configuration.GetRequiredSection(nameof(PostgresConfiguration)));
            builder.Services.Configure<CorsConfiguration>(builder.Configuration.GetRequiredSection(nameof(CorsConfiguration)));
            builder.Services.Configure<KeycloakConfiguration>(kcConfigurationSection);

            // -------------------------------------------------------------------------------------------------------------------------- >8

            var app = builder.Build();

            // -------------------------------------------------------------------------------------------------------------------------- >8

            app.UseForwardedHeaders();

            app.UseAuthentication();
            app.UseAuthorization();

            var origins = app.Services.GetRequiredService<IOptions<CorsConfiguration>>().Value.Origins;
            app.UseCors(o => o.AllowAnyMethod().AllowAnyHeader().WithOrigins(origins));

            app.MapGraphQL().RequireAuthorization();
            app.MapControllers().RequireAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapBananaCakePop().AllowAnonymous();
            }

            // -------------------------------------------------------------------------------------------------------------------------- >8

            using var scope = app.Services.CreateScope();

            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
            var db = scope.ServiceProvider.GetRequiredService<VocabContext>();

            db.Database.CanConnect().Throw(_ => new InvalidOperationException("Не получилось подключиться к базе данных.")).IfFalse();
            db.Database.EnsureCreated();
            // TODO: добавить проверку кейклока через http.

            // -------------------------------------------------------------------------------------------------------------------------- >8
            app.Run();
        }
    }
}
// https://github.com/edinSahbaz/clean-api-template?tab=readme-ov-file