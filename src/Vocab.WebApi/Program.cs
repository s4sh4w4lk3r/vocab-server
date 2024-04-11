using HotChocolate.Types;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using Throw;
using Vocab.Infrastructure.Configuration;
using Vocab.Infrastructure.GraphQL;
using Vocab.Infrastructure.Persistence;

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

            builder.Services.AddGraphQLServer()
            .ModifyOptions(options => { options.DefaultBindingBehavior = BindingBehavior.Implicit; })
            .AllowIntrospection(builder.Environment.IsDevelopment())
            .ModifyRequestOptions(o => o.OnlyAllowPersistedQueries = !builder.Environment.IsDevelopment())
            .UsePersistedQueryPipeline().AddReadOnlyFileSystemQueryStorage("./../Vocab.Infrastructure/GraphQL/PersistedQueries")
            .AddQueryType<Query>().AddProjections().AddFiltering().AddSorting().AddAuthorization();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Vocab WebApi",
                    Description = "Vocab WebApi"
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OpenIdConnect,
                    OpenIdConnectUrl = new Uri("http://localhost/auth/realms/vocab/.well-known/openid-configuration"),
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            // -------------------------------------------------------------------------------------------------------------------------- >8

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });

            builder.Services.Configure<PostgresConfiguration>(builder.Configuration.GetRequiredSection(nameof(PostgresConfiguration)));
            builder.Services.Configure<CorsConfiguration>(builder.Configuration.GetRequiredSection(nameof(CorsConfiguration)));

            // -------------------------------------------------------------------------------------------------------------------------- >8

            var app = builder.Build();

            // -------------------------------------------------------------------------------------------------------------------------- >8

            app.UseForwardedHeaders();

            /*app.UseAuthentication();
            app.UseAuthorization();*/

            var origins = app.Services.GetRequiredService<IOptions<CorsConfiguration>>().Value.Origins;
            app.UseCors(o => o.AllowAnyMethod().AllowAnyHeader().WithOrigins(origins));

            app.MapGraphQL()/*.RequireAuthorization()*/;
            app.MapControllers()/*.RequireAuthorization()*/;

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