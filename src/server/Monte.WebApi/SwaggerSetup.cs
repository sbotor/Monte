using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Monte.WebApi.Configuration;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Monte.WebApi;

public static class SwaggerSetup
{
    private const string SchemeId = "OIDC Auth Code with PKCE";
    
    public static void ConfigureSwagger(this IServiceCollection services, IConfigurationRoot config)
    {
        var settings = config.GetSection(nameof(TokenSettings)).Get<TokenSettings>()
            ?? throw new InvalidOperationException();
        
        services.AddSwaggerGen(x => ConfigureSwagger(x, settings.Issuer));
    }

    public static void UseConfiguredSwaggerUi(this WebApplication app)
        => app.UseSwagger().UseSwaggerUI(ConfigureSwaggerUi);

    private static void ConfigureSwagger(SwaggerGenOptions opt, Uri issuer)
    {
        var scheme = new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = HeaderNames.Authorization,
            Flows = new()
            {
                AuthorizationCode = new()
                {
                    AuthorizationUrl = new Uri(issuer, "connect/authorize"),
                    TokenUrl = new Uri(issuer, "connect/token"),
                }
            },
            Type = SecuritySchemeType.OAuth2
        };
        
        opt.AddSecurityDefinition("OIDC Auth Code with PKCE", scheme);
        
        opt.AddSecurityRequirement(new()
        {
            { 
                new()
                {
                    Reference = new()
                    {
                        Id = SchemeId,
                        Type = ReferenceType.SecurityScheme
                    }
                },
                Array.Empty<string>()
            }
        });
    }

    private static void ConfigureSwaggerUi(SwaggerUIOptions opt)
    {
        opt.OAuthClientId("swagger-ui");
        opt.OAuthScopes("openid roles profile monte_main_api");
        opt.OAuthUsePkce();
    }
}
