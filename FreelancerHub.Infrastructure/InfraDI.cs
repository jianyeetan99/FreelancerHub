using System.Text;
using FreelancerHub.Application.Interfaces;
using FreelancerHub.Application.Services;
using FreelancerHub.Domain.Constants;
using FreelancerHub.Domain.Models.AppSettings;
using FreelancerHub.Infrastructure.Common;
using FreelancerHub.Infrastructure.Repositories;
using FreelancerHub.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FreelancerHub.Infrastructure;

public static class InfraDI
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ConnectionStrings>(
            config.GetSection(AppSettingsSection.ConnectionStrings));
        services.AddSingleton<DapperContext>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IFreelancerService, FreelancerService>();
        services.AddScoped<IFreelancerRepository, FreelancerRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;

        // var connectionStrings = config.GetSection(nameof(ConnectionStrings)).Get<ConnectionStrings>();
        // var redisCacheSettings = config.GetSection(nameof(RedisCacheSettings)).Get<RedisCacheSettings>();
        //
        // services.AddStackExchangeRedisCache(options =>
        // {
        //     options.Configuration = connectionStrings?.Redis;
        //     options.InstanceName = redisCacheSettings?.InstanceName;
        // });
        //
        // services.Configure<ConnectionStrings>(config.GetSection(nameof(ConnectionStrings)));
        // services.Configure<RedisCacheSettings>(config.GetSection(nameof(RedisCacheSettings)));
        //
        // services.AddScoped<ICacheService, RedisCacheService>();
        // return services;
    }
}