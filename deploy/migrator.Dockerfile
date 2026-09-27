FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["VisoERP.DatabaseMigrator/VisoERP.DatabaseMigrator.csproj", "VisoERP.DatabaseMigrator/"]
COPY ["VisoERP.Domain/VisoERP.Domain.csproj", "VisoERP.Domain/"]
COPY ["VisoERP.Infra.Data/VisoERP.Infra.Data.csproj", "VisoERP.Infra.Data/"]
RUN dotnet restore "VisoERP.DatabaseMigrator/VisoERP.DatabaseMigrator.csproj"

COPY . .
RUN dotnet publish "VisoERP.DatabaseMigrator/VisoERP.DatabaseMigrator.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "VisoERP.DatabaseMigrator.dll"]
