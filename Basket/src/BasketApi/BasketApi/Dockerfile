FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 3180

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Basket/src/BasketApi/BasketApi/BasketApi.csproj", "Basket/src/BasketApi/BasketApi/"]
RUN dotnet restore "./Basket/src/BasketApi/BasketApi/./BasketApi.csproj"
COPY . .
WORKDIR "/src/Basket/src/BasketApi/BasketApi"
RUN dotnet build "./BasketApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BasketApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BasketApi.dll"]