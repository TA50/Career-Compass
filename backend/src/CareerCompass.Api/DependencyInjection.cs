using System.Text.Json;
using CareerCompass.Api.Controllers;
using CareerCompass.Api.OpenApi;
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
        services.AddAuthentication()
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
    }
}