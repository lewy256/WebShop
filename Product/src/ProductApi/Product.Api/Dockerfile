FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 3150

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Product/src/ProductApi/Product.Api/ProductApi.csproj", "Product/src/ProductApi/Product.Api/"]
COPY ["Product/src/ProductApi/ProductApi.Interfaces/ProductApi.Interfaces.csproj", "Product/src/ProductApi/ProductApi.Interfaces/"]
COPY ["Product/src/ProductApi/ProductApi.Models/ProductApi.Models.csproj", "Product/src/ProductApi/ProductApi.Models/"]
COPY ["Product/src/ProductApi/ProductApi.Shared.Models/ProductApi.Shared.Models.csproj", "Product/src/ProductApi/ProductApi.Shared.Models/"]
COPY ["Product/src/ProductApi/ProductApi.Service.Interfaces/ProductApi.Service.Interfaces.csproj", "Product/src/ProductApi/ProductApi.Service.Interfaces/"]
COPY ["Product/src/ProductApi/ProductApi.Services/ProductApi.Services.csproj", "Product/src/ProductApi/ProductApi.Services/"]
RUN dotnet restore "./Product/src/ProductApi/Product.Api/./ProductApi.csproj"
COPY . .
WORKDIR "/src/Product/src/ProductApi/Product.Api"
RUN dotnet build "./ProductApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductApi.dll"]