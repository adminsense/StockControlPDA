using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Components.Authorization;

using Microsoft.IdentityModel.Tokens;



namespace StockControl.Admin.Auth;



public static class AuthServiceExtensions

{

    public static IServiceCollection AddStockControlAuth(this IServiceCollection services, IConfiguration configuration)

    {

        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.SectionName));

        var jwtSection = configuration.GetSection(AuthOptions.SectionName);

        // Signing key for JWT after login — not the user password. Login uses users.Username + users.Password (hash) in SQL.
        var key = jwtSection["Key"];
        if (string.IsNullOrWhiteSpace(key))
            key = "StockControl-App-Jwt-Signing-Key-Min32Chars";



        services.PostConfigure<AuthOptions>(o => o.Key = key);



        services.AddSingleton<PasswordHasherService>();

        services.AddScoped<UserAuthService>();

        services.AddSingleton<JwtTokenService>();

        services.AddHttpContextAccessor();

        services.AddScoped<ServerAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<ServerAuthenticationStateProvider>());

        services.AddCascadingAuthenticationState();



        var authOptions = new AuthOptions

        {

            Key = key,

            Issuer = jwtSection["Issuer"] ?? "StockControl",

            Audience = jwtSection["Audience"] ?? "StockControl",

            ExpiryHours = int.TryParse(jwtSection["ExpiryHours"], out var h) ? h : 12

        };

        var validationParameters = AuthJwt.CreateValidationParameters(authOptions);



        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

            .AddJwtBearer(options =>

            {

                options.TokenValidationParameters = validationParameters;

                options.Events = new JwtBearerEvents

                {

                    OnMessageReceived = context =>

                    {

                        if (context.Request.Cookies.TryGetValue(AuthConstants.JwtCookieName, out var cookieToken))

                            context.Token = cookieToken;

                        return Task.CompletedTask;

                    }

                };

            });



        services.AddAuthorization(options =>

        {

            options.AddPolicy("AdminOnly", p => p.RequireRole(AuthConstants.AdminRole));

            options.AddPolicy("PdaOnly", p => p.RequireRole(AuthConstants.AdminPdaRole));

            // No FallbackPolicy: _Host + BlazorHub are anonymous; App.razor AuthorizeView shows LoginModal.

        });



        return services;

    }

}


