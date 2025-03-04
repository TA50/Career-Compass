FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/CareerCompass.Api/CareerCompass.Api.csproj", "src/CareerCompass.Api/"]
COPY ["src/CareerCompass.Aspire.ServiceDefaults/CareerCompass.Aspire.ServiceDefaults.csproj", "src/CareerCompass.Aspire.ServiceDefaults/"]
COPY ["src/CareerCompass.Core/CareerCompass.Core.csproj", "src/CareerCompass.Core/"]
COPY ["src/CareerCompass.Infrastructure/CareerCompass.Infrastructure.csproj", "src/CareerCompass.Infrastructure/"]
RUN dotnet restore "src/CareerCompass.Api/CareerCompass.Api.csproj"
COPY src .
WORKDIR "/src/CareerCompass.Api"
RUN dotnet build "CareerCompass.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CareerCompass.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["cert.pfx", "cert.pfx"]
ENTRYPOINT ["dotnet", "CareerCompass.Api.dll"]
