FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 3191

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ReverseProxy/ReverseProxy/ReverseProxy.csproj", "ReverseProxy/ReverseProxy/"]
RUN dotnet restore "./ReverseProxy/ReverseProxy/./ReverseProxy.csproj"
COPY . .
WORKDIR "/src/ReverseProxy/ReverseProxy"
RUN dotnet build "./ReverseProxy.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ReverseProxy.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReverseProxy.dll"]