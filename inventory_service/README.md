# Inventory Service (Onion Architecture)

This service was refactored to follow Onion Architecture with clear separation of concerns and dependency flow.

## Structure

- Inventory.Domain
  - Entities: core domain models (Inventory, Store)
- Inventory.Application
  - DTOs, Mappers
  - Abstractions: Repository interfaces, external clients
  - Services: business logic (InventoryService, StoreService)
- Inventory.Infrastructure
  - Data: EF Core ApplicationDbContext (PostgreSQL)
  - Repositories: concrete implementations
  - Clients: ProductCatalogClient (HTTP)
  - Messaging: KafkaHostedService (consumer/producer)
- Inventory.Api (Web API)
  - Controllers, Program, Exception handling, extensions

Dependencies point inward only:
Web (Inventory.Api) -> Application, Infrastructure
Infrastructure -> Application, Domain
Application -> Domain
Domain -> none

## Design patterns

- Repository pattern for persistence
- Gateway client for Product Catalog API
- Background Service for Kafka consumer/producer

## How to run

Prerequisites:
- .NET 8 SDK (installed)
- PostgreSQL connection string in appsettings.json (DefaultConnection)
- Optional: Kafka broker at localhost:7092 (if not running, the app still starts but Kafka will log connection errors)

Run the API:

```bash
# from inventory_service folder
dotnet build inventory_service.sln
DOTNET_ENVIRONMENT=Development dotnet run --project Inventory.Api/Inventory.Api.csproj
```

Swagger will be available in Development at /swagger.

## Notes

- EF Core migrations now live in `Inventory.Infrastructure/Migrations`. The app applies pending migrations on startup.
- If you need to add a new migration:

```bash
~/.dotnet/tools/dotnet-ef migrations add <Name> \
  -p Inventory.Infrastructure/Inventory.Infrastructure.csproj \
  -s Inventory.Api/Inventory.Api.csproj \
  -o Migrations
```

- You can delete the legacy `Migrations/` folder in the old `inventory_service` project (it is no longer used).
- The old `inventory_service` project has been superseded by `Inventory.Api` and was removed from the solution. The files remain for reference and can be deleted later.
- Configure `ProductCatalogService:BaseUrl` in `appsettings.json`.
- Kafka topics are ensured on startup; adjust bootstrap servers via configuration if needed.
