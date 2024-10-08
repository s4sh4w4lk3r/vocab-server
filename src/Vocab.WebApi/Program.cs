﻿using Hangfire;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Common;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Throw;
using Vocab.Application.Abstractions.Services;
using Vocab.Infrastructure.Persistence;
using Vocab.Infrastructure.Services;
using Vocab.WebApi.Extensions;

[assembly: ApiController]

namespace Vocab.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region App Builder

            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var configuration = builder.Configuration;
            bool isDevelopment = builder.Environment.IsDevelopment();

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

            // -------------------------------------------------------------------------------------------------------------------------- >8

            services.AddControllers();
            services.AddVocabDbContext(connectionString: configuration.GetConnectionString("PostgreSql")!,
                sensitiveDataLoggingEnabled: isDevelopment);

            services.AddKeycloakWebApiAuthentication(configuration, jwt=> jwt.TokenValidationParameters.ValidIssuers = configuration.GetJwtValidIssuers());

            services.AddAuthorization();
            Uri kcUri = new(configuration.GetKeycloakOptions<KeycloakAuthenticationOptions>()
                ?.KeycloakUrlRealm.ThrowIfNull().IfEmpty().IfWhiteSpace().Value!);

            services.AddSwaggerVocab(kcUri);
            services.AddVocabHangfire(configuration);
            services.AddHealthChecks().AddDbContextCheck<VocabContext>();


            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IStatementDictionaryService, StatementDictionaryService>();
            services.AddScoped<IStatementPairService, StatementPairService>();
            services.AddScoped<IChallengeService, ChallengeService>();
            services.AddScoped<IStatementsImportService, StatementsImportService>();

            #endregion
            // -------------------------------------------------------------------------------------------------------------------------- >8
            #region App

            var app = builder.Build();

            app.UseForwardedHeaders();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard();


            app.UseCors(o => o
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins(configuration.GetCorsOrigins()));

            app.UseWebSockets();
            app.MapControllers().RequireAuthorization();
            app.MapHangfireDashboard();
            app.MapHealthChecks("/healthz").RequireHost("localhost:8080").AllowAnonymous();

            if (isDevelopment is true)
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // -------------------------------------------------------------------------------------------------------------------------- >8
            app.Run();
            #endregion
        }
    }
}
// https://github.com/edinSahbaz/clean-api-template?tab=readme-ov-file
#warning разобраться с логгированием в докере