# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Api/ECommerce.Api/ECommerce.Api.csproj", "src/Api/ECommerce.Api/"]
COPY ["src/Modules/Catalog/ECommerce.Modules.Catalog.Infrastructure/ECommerce.Modules.Catalog.Infrastructure.csproj", "src/Modules/Catalog/ECommerce.Modules.Catalog.Infrastructure/"]
COPY ["src/Common/ECommerce.Common.Infrastructure/ECommerce.Common.Infrastructure.csproj", "src/Common/ECommerce.Common.Infrastructure/"]
COPY ["src/Common/ECommerce.Common.Application/ECommerce.Common.Application.csproj", "src/Common/ECommerce.Common.Application/"]
COPY ["src/Common/ECommerce.Common.Domain/ECommerce.Common.Domain.csproj", "src/Common/ECommerce.Common.Domain/"]
COPY ["src/Modules/Catalog/ECommerce.Modules.Catalog.Application/ECommerce.Modules.Catalog.Application.csproj", "src/Modules/Catalog/ECommerce.Modules.Catalog.Application/"]
COPY ["src/Modules/Catalog/ECommerce.Modules.Catalog.Domain/ECommerce.Modules.Catalog.Domain.csproj", "src/Modules/Catalog/ECommerce.Modules.Catalog.Domain/"]
COPY ["src/Modules/Catalog/ECommerce.Modules.Catalog.Presentation/ECommerce.Modules.Catalog.Presentation.csproj", "src/Modules/Catalog/ECommerce.Modules.Catalog.Presentation/"]
COPY ["src/Common/ECommerce.Common.Presentation/ECommerce.Common.Presentation.csproj", "src/Common/ECommerce.Common.Presentation/"]
COPY ["src/Modules/Ticketing/ECommerce.Modules.Ticketing.Infrastructure/ECommerce.Modules.Ticketing.Infrastructure.csproj", "src/Modules/Ticketing/ECommerce.Modules.Ticketing.Infrastructure/"]
COPY ["src/Modules/Ticketing/ECommerce.Modules.Ticketing.Application/ECommerce.Modules.Ticketing.Application.csproj", "src/Modules/Ticketing/ECommerce.Modules.Ticketing.Application/"]
COPY ["src/Modules/Ticketing/ECommerce.Modules.Ticketing.Domain/ECommerce.Modules.Ticketing.Domain.csproj", "src/Modules/Ticketing/ECommerce.Modules.Ticketing.Domain/"]
COPY ["src/Modules/Ticketing/ECommerce.Modules.Ticketing.IntegrationEvents/ECommerce.Modules.Ticketing.IntegrationEvents.csproj", "src/Modules/Ticketing/ECommerce.Modules.Ticketing.IntegrationEvents/"]
COPY ["src/Modules/Ticketing/ECommerce.Modules.Ticketing.Presentation/ECommerce.Modules.Ticketing.Presentation.csproj", "src/Modules/Ticketing/ECommerce.Modules.Ticketing.Presentation/"]
COPY ["src/Modules/Users/ECommerce.Modules.Users.IntegrationEvents/ECommerce.Modules.Users.IntegrationEvents.csproj", "src/Modules/Users/ECommerce.Modules.Users.IntegrationEvents/"]
COPY ["src/Modules/Users/ECommerce.Modules.Users.Infrastructure/ECommerce.Modules.Users.Infrastructure.csproj", "src/Modules/Users/ECommerce.Modules.Users.Infrastructure/"]
COPY ["src/Modules/Users/ECommerce.Modules.Users.Application/ECommerce.Modules.Users.Application.csproj", "src/Modules/Users/ECommerce.Modules.Users.Application/"]
COPY ["src/Modules/Users/ECommerce.Modules.Users.Domain/ECommerce.Modules.Users.Domain.csproj", "src/Modules/Users/ECommerce.Modules.Users.Domain/"]
COPY ["src/Modules/Users/ECommerce.Modules.Users.Presentation/ECommerce.Modules.Users.Presentation.csproj", "src/Modules/Users/ECommerce.Modules.Users.Presentation/"]
COPY ["src/Modules/Webhooks/ECommerce.Webhooks.Infrastructure/ECommerce.Webhooks.Infrastructure.csproj", "src/Modules/Webhooks/ECommerce.Webhooks.Infrastructure/"]
COPY ["src/Modules/Webhooks/ECommerce.Webhooks.Application/ECommerce.Webhooks.Application.csproj", "src/Modules/Webhooks/ECommerce.Webhooks.Application/"]
COPY ["src/Modules/Webhooks/ECommerce.Webhooks.Domain/ECommerce.Webhooks.Domain.csproj", "src/Modules/Webhooks/ECommerce.Webhooks.Domain/"]
COPY ["src/Modules/Webhooks/ECommerce.Webhooks.Presentation/ECommerce.Webhooks.Presentation.csproj", "src/Modules/Webhooks/ECommerce.Webhooks.Presentation/"]
RUN dotnet restore "./src/Api/ECommerce.Api/ECommerce.Api.csproj"
COPY . .
WORKDIR "/src/src/Api/ECommerce.Api"
RUN dotnet build "./ECommerce.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ECommerce.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ECommerce.Api.dll"]