FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["QuantityMeasurementWebAPI.csproj", "./"]
COPY ["../QuantityMeasurementApp.Service/QuantityMeasurementApp.Service.csproj", "../QuantityMeasurementApp.Service/"]
COPY ["../QuantityMeasurementApp.Repository/QuantityMeasurementApp.Repository.csproj", "../QuantityMeasurementApp.Repository/"]
COPY ["../QuantityMeasurementApp.Entity/QuantityMeasurementApp.Entity.csproj", "../QuantityMeasurementApp.Entity/"]

RUN dotnet restore "QuantityMeasurementWebAPI.csproj"

COPY . .
WORKDIR "/src"

RUN dotnet publish "QuantityMeasurementWebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "QuantityMeasurementWebAPI.dll"]
