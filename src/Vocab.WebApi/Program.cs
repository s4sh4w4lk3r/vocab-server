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
            var builder = WebApplication.CreateBuilder(args);

            // -------------------------------------------------------------------------------------------------------------------------- >8

            builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console()
            .WriteTo.File($"./logs/log.log", rollingInterval: RollingInterval.Day)
            .ReadFrom.Configuration(ctx.Configuration));

            // -------------------------------------------------------------------------------------------------------------------------- >8

            IConfigurationSection DatabaseConfigurationSection = builder.Configuration.GetRequiredSection(nameof(DatabaseConfiguration));

            DatabaseConfiguration databaseConfiguration = DatabaseConfigurationSection.Get<DatabaseConfiguration>() ?? throw new ArgumentNullException("Конфигурация базы данных не получена.");

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });

            builder.Services.Configure<DatabaseConfiguration>(DatabaseConfigurationSection);
            builder.Services.Configure<CorsConfiguration>(builder.Configuration.GetRequiredSection(nameof(CorsConfiguration)));

            // -------------------------------------------------------------------------------------------------------------------------- >8

            builder.Services.AddControllers();
            builder.Services.AddDbContext<VocabContext>(options =>
            {
                options.EnableSensitiveDataLogging(sensitiveDataLoggingEnabled: builder.Environment.IsDevelopment());
                options.UseSqlServer(databaseConfiguration.ConnectionString, options => options.MigrationsAssembly("Vocab.WebApi"));
            },
            contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Scoped);

            builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
            builder.Services.AddAuthorization();

            builder.Services.AddSwaggerVocab(new Uri(kcConfiguration.MetadataAddress));

            builder.Services.AddScoped<IRatingService, RatingService>();
            builder.Services.AddScoped<IStatementDictionaryService, StatementDictionaryService>();
            builder.Services.AddScoped<IStatementPairService, StatementPairService>();

            // -------------------------------------------------------------------------------------------------------------------------- >8

            var app = builder.Build();

            // -------------------------------------------------------------------------------------------------------------------------- >8

            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VocabContext>();
            var origins = scope.ServiceProvider.GetRequiredService<IOptions<CorsConfiguration>>().Value.Origins;

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

            db.Database.CanConnect().Throw(_ => new InvalidOperationException("�� ���������� ������������ � ���� ������.")).IfFalse();
            using (HttpClient httpClient = new())
            {
                Uri uri = new(kcConfiguration.MetadataAddress);
                string kcExceptionMessage = "�� ���������� ������������ � ������� ��������������.";
                try
                {
                    bool isSuccess = (await httpClient.GetAsync(uri)).IsSuccessStatusCode;
                    isSuccess.Throw(_ => new InvalidOperationException(kcExceptionMessage)).IfFalse();
                }
                catch (HttpRequestException ex)
                {
                    throw new InvalidOperationException(kcExceptionMessage, ex);
                }

            }

            // -------------------------------------------------------------------------------------------------------------------------- >8
            app.Run();
        }
    }
}
// https://github.com/edinSahbaz/clean-api-template?tab=readme-ov-file