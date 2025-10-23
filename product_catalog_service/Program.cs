using product_catalog_service.Data;
using product_catalog_service.Data.Interfaces;
using product_catalog_service.Repositories;
using product_catalog_service.Services;
using product_catalog_service.Settings;
using product_catalog_service.ExceptionHandlers;
using Microsoft.AspNetCore.Mvc;
using product_catalog_service.Models;
using product_catalog_service.Extensions;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddSingleton<IMongoDbContext, MongoDBContext>();
builder.Services.AddSingleton<IMongoIndexService, MongoIndexService>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddHttpClient<ITemplateService, TemplateService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:8080/");
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var err = context.ModelState.ToErrorResponse();
        var result = new BadRequestObjectResult(err);
        result.ContentTypes.Add("application/json");
        return result;
    };
});
var app = builder.Build();

// Ensure indexes on startup
using (var scope = app.Services.CreateScope())
{
    var indexService = scope.ServiceProvider.GetService<IMongoIndexService>();
    if (indexService != null)
    {
        // fire-and-forget - but wait a short time to finish during startup
        indexService.EnsureIndexesAsync().GetAwaiter().GetResult();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
