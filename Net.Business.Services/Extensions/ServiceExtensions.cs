using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Net.Connection;
using Net.Data;

namespace Net.Business.Services
{
    public static class ServiceExtensions
    {

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins("http://localhost:4201,http://131.107.40.220:8083")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowAnyOrigin());

                //opt.AddPolicy("CorsPolicy",
                //    builder => builder.WithOrigins("http://131.107.40.220:8083")
                //                      .AllowAnyHeader()
                //                      .AllowAnyMethod()
                //                      .AllowAnyOrigin());
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
            });
        }

        public static void ConfigureSQLConnection(this IServiceCollection services)
        {
            services.AddScoped<IConnectionSQL, ConnectionSQL>();
        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

            //services.AddSingleton<DriveApiService>();
        }
    }
}
