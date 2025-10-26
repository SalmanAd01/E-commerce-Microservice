using Inventory.Infrastructure.Data;
using Inventory.Api.ExceptionHandlers;
using Inventory.Api.Extensions;
using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Services;
using Inventory.Application.Services.Contracts;
using Inventory.Infrastructure.Repositories;
using Inventory.Infrastructure.Clients;
using Inventory.Infrastructure.Messaging;
using Inventory.Infrastructure.Messaging.Handlers;
using Inventory.Infrastructure.Configuration;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<ProductCatalogClient>();
builder.Services.AddScoped<Inventory.Application.Abstractions.Clients.IProductCatalogClient, ProductCatalogClient>();

builder.Services.AddHttpClient("product", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ProductCatalogService:BaseUrl")!);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails();
// Options pattern for Kafka
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));
// FluentValidation pipeline
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Inventory.Application.Validation.CreateInventoryDtoValidator>();
// Kafka message handlers (Strategy pattern)
builder.Services.AddSingleton<IKafkaMessageHandler, OrderCreatedHandler>();
builder.Services.AddSingleton<IKafkaMessageHandler, OrderCancelledHandler>();
builder.Services.AddSingleton<IKafkaClientFactory, KafkaClientFactory>();
builder.Services.AddHostedService<KafkaHostedService>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
        logger.LogError(ex, "Failed to apply EF Core migrations on startup");
    }
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
