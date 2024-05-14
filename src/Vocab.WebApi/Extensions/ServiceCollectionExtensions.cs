using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Vocab.Infrastructure.Configuration;

namespace Vocab.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
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
        public static void AddAuthenticationVocab(this IServiceCollection services, KeycloakConfiguration keycloakConfiguration)
        {
            TokenValidationParameters validationParameters = new()
            {
                ClockSkew = TimeSpan.FromMinutes(5),
                ValidateAudience = true,
                ValidateIssuer = true,
                NameClaimType = "preferred_username",
                //RoleClaimType = "role",
                ValidIssuers = keycloakConfiguration.ValidIssuers,
                ValidAudiences = ["account", "realm-management"]
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
               {
                   opts.Audience = "aspnet";
                   opts.TokenValidationParameters = validationParameters;
                   opts.RequireHttpsMetadata = false;
                   opts.SaveToken = true;
                   opts.MetadataAddress = keycloakConfiguration.MetadataAddress;
               });


        }
        public static void AddAuthorizationVocab(this IServiceCollection services, KeycloakConfiguration keycloakConfiguration)
        {


        }
    }
}
