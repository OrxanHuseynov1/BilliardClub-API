using BusinessLayer.ExternalServices.Abstractions;
using BusinessLayer.ExternalServices.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using FluentValidation.AspNetCore;
using FluentValidation;
using BusinessLayer.Profiles.CompanyProfiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using BusinessLayer.Services.Implementations;
using BusinessLayer.Services.Abstractions;

namespace BusinessLayer;

public static class BLRegistration
{
    public static void AddBlServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddAutoMapper(typeof(CompanyProfile).Assembly);
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ITelegramService, TelegramService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITableService, TableService>();
        services.AddScoped<ITablesPriceService, TablesPriceService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITableSessionService, TableSessionService>();
        services.AddScoped<ISessionProductService, SessionProductService>();
        services.AddScoped<IExpensesService, ExpensesService>();

        services.AddAuthentication(cfg => {
            cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x => {

            x.TokenValidationParameters = new TokenValidationParameters
            {

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8
                        .GetBytes(configuration["Jwt:SecretKey"]!)
                ),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidIssuer = configuration["Jwt:Issuer"]
            };
        });
    }
}

