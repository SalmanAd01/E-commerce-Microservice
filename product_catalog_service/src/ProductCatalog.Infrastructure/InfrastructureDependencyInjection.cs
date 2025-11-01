using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Application.Abstractions.External;
using ProductCatalog.Application.Abstractions.Repositories;
using ProductCatalog.Infrastructure.Data;
using ProductCatalog.Infrastructure.External;
using ProductCatalog.Infrastructure.Repositories;
using ProductCatalog.Infrastructure.Settings;
using CategoryTemplate.Grpc;
using Grpc.Net.ClientFactory;

namespace ProductCatalog.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbSettings>(configuration.GetSection("ConnectionStrings"));
            services.AddSingleton<IMongoDbContext, MongoDBContext>();
            services.AddSingleton<IMongoIndexService, MongoIndexService>();

            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            var useGrpc = bool.TryParse(configuration["TemplateService:UseGrpc"], out var u) && u;
            if (useGrpc)
            {
                var address = configuration["TemplateService:Grpc:Address"] ?? "http://localhost:9090";
                services.AddGrpcClient<CategoryTemplateService.CategoryTemplateServiceClient>(o =>
                {
                    o.Address = new Uri(address);
                });
                services.AddScoped<ITemplateService, TemplateGrpcService>();
            }
            else
            {
                var templateBaseUrl = configuration["TemplateService:BaseUrl"] ?? "http://localhost:8080/";
                services.AddHttpClient<ITemplateService, TemplateHttpService>(client =>
                {
                    client.BaseAddress = new Uri(templateBaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(10);
                });
            }

            return services;
        }
    }
}
