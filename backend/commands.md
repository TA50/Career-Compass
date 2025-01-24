## Add New Migration

```bash
dotnet ef migrations add --project CareerCompass.Infrastructure/CareerCompass.Infrastructure.csproj --startup-project CareerCompass.Api/CareerCompass.Api.csproj --context CareerCompass.Infrastructure.Persistence.AppDbContext --configuration Debug SplitAgentDetails --output-dir Migrations
```

## Update Database

```bash

dotnet ef database update --project CareerCompass.Infrastructure/CareerCompass.Infrastructure.csproj --startup-project CareerCompass.Api/CareerCompass.Api.csproj --context CareerCompass.Infrastructure.Persistence.AppDbContext --configuration Debug  <Name>

```