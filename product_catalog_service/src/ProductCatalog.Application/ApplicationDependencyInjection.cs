using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Application.Services;

namespace ProductCatalog.Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IProductService, ProductService>();
            return services;
        }
    }
}
