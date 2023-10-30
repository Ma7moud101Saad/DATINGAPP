using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services,IConfiguration config) {

            services.AddDbContext<DataContext>(option =>
            option.UseNpgsql(config["ConnectionStrings:PostgresConnection"]));
            services.AddScoped<ITokenService, TokenService>();
            services.AddCors();
            return services;
        }
    }
}
