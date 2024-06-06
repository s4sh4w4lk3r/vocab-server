﻿using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Throw;
using Vocab.Infrastructure.Persistence;

namespace Vocab.WebApi.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static void AddSwaggerVocab(this IServiceCollection services, Uri openIdConnectUrl)
        {
            services.AddSwaggerGen(options =>
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
                    OpenIdConnectUrl = openIdConnectUrl,
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
        }

        public static void AddVocabDbContext(this IServiceCollection services, string connectionString, bool sensitiveDataLoggingEnabled)
        {
            services.AddDbContext<VocabContext>(options =>
            {
                connectionString.ThrowIfNull().IfEmpty().IfWhiteSpace();

                options.EnableSensitiveDataLogging(sensitiveDataLoggingEnabled: sensitiveDataLoggingEnabled);
                options.UseSqlServer(connectionString, options => options.MigrationsAssembly("Vocab.WebApi"));
            },
           contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Scoped);
        }
    }
}
