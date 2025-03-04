using System.Text.Json;
using CareerCompass.Api.Controllers;
using CareerCompass.Api.OpenApi;
using CareerCompass.Api.Services;
using CareerCompass.Core.Common;
using CareerCompass.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace CareerCompass.Api;

public static class DependencyInjection
{
    public static void AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi(options => { options.AddDocumentTransformer<AppOpenApiDocumentTransformer>(); });


        #region Auth

        services.AddAuthorization();
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                var expireTimeInMinutes = configuration.GetValue<int>("Authentication:Cookie:ExpireTimeInMinutes");
                var expireTimeSpan = TimeSpan.FromMinutes(expireTimeInMinutes);
                options.ExpireTimeSpan = expireTimeSpan;
                options.SlidingExpiration = true;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    var res = JsonSerializer.Serialize(new
                    {
                        Message = "Unauthorized"
                    });
                    return context.Response.WriteAsync(res);
                };

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;

                    var res = JsonSerializer.Serialize(new
                    {
                        Message = "Forbidden"
                    });
                    return context.Response.WriteAsync(res);
                };

                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.MaxAge = expireTimeSpan;
            });

        #endregion

        services.AddControllers(o =>
        {
            List<IAuthorizeData> authorizeData = [new AuthorizeAttribute()];
            o.Filters.Add(new AuthorizeFilter(authorizeData));
        });


        // Services
        services.AddScoped<ApiControllerContext>();
        services.AddAutoMapper(typeof(Program));
        services.AddTransient<AuthenticationEmailSender>();
    }


    public static void ConfigureSettings(this WebApplicationBuilder builder)
    {
        builder.ConfigureSettings<SmtpSettings>(nameof(SmtpSettings));
        builder.ConfigureSettings<CoreSettings>(nameof(CoreSettings));
    }

    private static void ConfigureSettings<TOptions>(this WebApplicationBuilder builder, string sectionName)
        where TOptions : class, new()
    {
        var section = builder.Configuration.GetSection(sectionName);
        if (!section.Exists()) return;
        
        var options = new TOptions();
        section.Bind(options);
        builder.Services.AddSingleton(options);
    }
}