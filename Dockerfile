# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy all project files first (for layer caching of restore)
COPY QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj             QuantityMeasurementWebAPI/
COPY QuantityMeasurementApp.Service/QuantityMeasurementApp.Service.csproj   QuantityMeasurementApp.Service/
COPY QuantityMeasurementApp.Repository/QuantityMeasurementApp.Repository.csproj QuantityMeasurementApp.Repository/
COPY QuantityMeasurementApp.Entity/QuantityMeasurementApp.Entity.csproj     QuantityMeasurementApp.Entity/

# Restore dependencies
RUN dotnet restore QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj

# Copy all source code
COPY . .

# Publish in Release mode
RUN dotnet publish QuantityMeasurementWebAPI/QuantityMeasurementWebAPI.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install native dependencies for Npgsql (GSSAPI/Kerberos)
RUN apt-get update && \
    apt-get install -y --no-install-recommends libgssapi-krb5-2 && \
    rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Render / cloud platforms inject PORT env variable; fall back to 8080
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
EXPOSE 8080

ENTRYPOINT ["dotnet", "QuantityMeasurementWebAPI.dll"]
