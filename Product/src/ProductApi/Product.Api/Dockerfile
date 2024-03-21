FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 3150

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Product.Api/ProductApi.csproj", "ProductApi/Product.Api/"]
COPY ["ProductApi.Interfaces/ProductApi.Interfaces.csproj", "ProductApi/ProductApi.Interfaces/"]
COPY ["ProductApi.Models/ProductApi.Models.csproj", "ProductApi/ProductApi.Models/"]
COPY ["ProductApi.Shared.Models/ProductApi.Shared.Models.csproj", "ProductApi/ProductApi.Shared.Models/"]
COPY ["ProductApi.Service.Interfaces/ProductApi.Service.Interfaces.csproj", "ProductApi/ProductApi.Service.Interfaces/"]
COPY ["ProductApi.Services/ProductApi.Services.csproj", "ProductApi/ProductApi.Services/"]
RUN dotnet restore "ProductApi/Product.Api/ProductApi.csproj"

WORKDIR "/src/ProductApi"
COPY . .

RUN dotnet build "Product.Api/ProductApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Product.Api/ProductApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductApi.dll"]