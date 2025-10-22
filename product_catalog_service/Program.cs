using product_catalog_service.Data;
using product_catalog_service.Data.Interfaces;
using product_catalog_service.Repositories;
using product_catalog_service.Services;
using product_catalog_service.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
var settings = builder.Configuration.GetSection("ConnectionStrings").Get<MongoDbSettings>()!;
builder.Services.AddSingleton(settings);
builder.Services.AddSingleton<IMongoDbContext, MongoDBContext>();

builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
