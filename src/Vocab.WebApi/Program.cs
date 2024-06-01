using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Common;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Throw;
using Vocab.Application.Abstractions.Services;
using Vocab.Infrastructure.Configuration;
using Vocab.Infrastructure.Persistence;
using Vocab.Infrastructure.Services;
using Vocab.WebApi.Extensions;

[assembly: ApiController]

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

            // -------------------------------------------------------------------------------------------------------------------------- >8

            builder.Host.UseSerilog((ctx, loggerConfiguration) =>
            {
                string template = "[{Timestamp:HH:mm:ss} {Level:u3} {TraceId}] {Message:lj}{NewLine}{Exception}";
                loggerConfiguration.WriteTo.Console(outputTemplate: template);
                loggerConfiguration.WriteTo.File(path: $"./logs/log.log", rollingInterval: RollingInterval.Day, outputTemplate: template);
                loggerConfiguration.ReadFrom.Configuration(ctx.Configuration);
            });


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

            Uri kcUri;
            {
                string kcUrlStr = configuration.GetKeycloakOptions<KeycloakAuthenticationOptions>()?.KeycloakUrlRealm.ThrowIfNull().IfEmpty().IfWhiteSpace().Value!;
                kcUri = new(kcUrlStr);
            }

            services.AddSwaggerVocab(kcUri);

            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IStatementDictionaryService, StatementDictionaryService>();
            services.AddScoped<IStatementPairService, StatementPairService>();
            services.AddScoped<IChallengeService, ChallengeService>();

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