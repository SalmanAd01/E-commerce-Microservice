using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application;
using ProductCatalog.Domain.Common;
using ProductCatalog.Infrastructure;
using ProductCatalog.Infrastructure.Data;
using ProductCatalog.Api.ExceptionHandlers;
using ProductCatalog.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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
        indexService.EnsureIndexesAsync().GetAwaiter().GetResult();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
