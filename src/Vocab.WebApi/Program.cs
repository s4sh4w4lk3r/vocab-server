using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Throw;
using Vocab.Application.Abstractions.Services;
using Vocab.Infrastructure.Configuration;
using Vocab.Infrastructure.Persistence;
using Vocab.Infrastructure.Services;
using Vocab.WebApi.Extensions;

namespace Vocab.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            #region App Builder

            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var configuration = builder.Configuration;

#warning убрать из сурса ссылку
            Uri kcUri = new("http://auth.vocab.rlx/realms/vocab/.well-known/openid-configuration");

            // -------------------------------------------------------------------------------------------------------------------------- >8

            builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console()
            .WriteTo.File($"./logs/log.log", rollingInterval: RollingInterval.Day)
            .ReadFrom.Configuration(ctx.Configuration));

            // -------------------------------------------------------------------------------------------------------------------------- >8

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });

            services.Configure<CorsConfiguration>(configuration.GetRequiredSection(nameof(CorsConfiguration)));

            // -------------------------------------------------------------------------------------------------------------------------- >8

            services.AddControllers();
            services.AddDbContext<VocabContext>(options =>
            {
                string connString = configuration.GetConnectionString("SqlServer")
                .ThrowIfNull().IfEmpty().IfWhiteSpace().Value;

                options.EnableSensitiveDataLogging(sensitiveDataLoggingEnabled: builder.Environment.IsDevelopment());
                options.UseSqlServer(connString, options => options.MigrationsAssembly("Vocab.WebApi"));
            },
            contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Scoped);

            services.AddKeycloakWebApiAuthentication(configuration);
            services.AddAuthorization();

            services.AddSwaggerVocab(kcUri);

            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IStatementDictionaryService, StatementDictionaryService>();
            services.AddScoped<IStatementPairService, StatementPairService>();

            #endregion
            // -------------------------------------------------------------------------------------------------------------------------- >8
            #region App

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VocabContext>();
            var origins = scope.ServiceProvider.GetRequiredService<IOptions<CorsConfiguration>>().Value.Origins ?? throw new NullReferenceException("Конфигурация CORS не получена.");

            app.UseForwardedHeaders();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(o => o.AllowAnyMethod().AllowAnyHeader().WithOrigins(origins));

            app.UseWebSockets();
            app.MapControllers().RequireAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                //db.Database.EnsureCreated();
            }

            // -------------------------------------------------------------------------------------------------------------------------- >8

            db.Database.CanConnect().Throw(_ => new InvalidOperationException("Не получилось подключиться к базе данных.")).IfFalse();
            using (HttpClient httpClient = new())
            {
                string kcExceptionMessage = "Не получилось подключиться к серверу аутентификации.";
                try
                {
                    bool isSuccess = (await httpClient.GetAsync(kcUri)).IsSuccessStatusCode;
                    isSuccess.Throw(_ => new InvalidOperationException(kcExceptionMessage)).IfFalse();
                }
                catch (HttpRequestException ex)
                {
                    throw new InvalidOperationException(kcExceptionMessage, ex);
                }

            }

            // -------------------------------------------------------------------------------------------------------------------------- >8
            app.Run(); 
            #endregion
        }
    }
}
// https://github.com/edinSahbaz/clean-api-template?tab=readme-ov-file