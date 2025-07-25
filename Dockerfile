# Imagen base para aplicaciones ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Imagen para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia todo el c�digo
COPY . .

# Restaura dependencias
RUN dotnet restore

# Compila y publica el proyecto
RUN dotnet publish -c Release -o /app/publish

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Aqu� pon el nombre real de tu DLL
ENTRYPOINT ["dotnet", "ZetaDashboard.dll"]