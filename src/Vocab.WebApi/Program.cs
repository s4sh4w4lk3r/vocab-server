﻿using Hangfire;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Common;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.RegularExpressions;
using Throw;
using Vocab.Application.Abstractions.Services;
using Vocab.Infrastructure.Persistence;
using Vocab.Infrastructure.Services;
using Vocab.WebApi.Extensions;

[assembly: ApiController]

namespace Vocab.WebApi
{
    public partial class Program
    {
        public static async Task Main(string[] args)
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

            services.AddKeycloakWebApiAuthentication(configuration);
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


            string? corsStr = configuration.GetRequiredSection("Cors:Origins").Value;
            string[] corsArr = !string.IsNullOrEmpty(corsStr) ? CorsOriginsSplitPattern().Matches(corsStr).Select(x => x.Value).ToArray() : [];

            app.UseCors(o => o
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins(corsArr));

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

        #region RegexCodeGenerators
        [GeneratedRegex(@"((?>[\w:\/.]+))", RegexOptions.Multiline)]
        private static partial Regex CorsOriginsSplitPattern(); 
        #endregion
    }
}
// https://github.com/edinSahbaz/clean-api-template?tab=readme-ov-file
#warning разобраться с логгированием в докере, подружить kc