FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["VisoERP.Web/VisoERP.Web.csproj", "VisoERP.Web/"]
COPY ["VisoERP.Application/VisoERP.Application.csproj", "VisoERP.Application/"]
COPY ["VisoERP.Domain/VisoERP.Domain.csproj", "VisoERP.Domain/"]
COPY ["VisoERP.Infra.Auth/VisoERP.Infra.Auth.csproj", "VisoERP.Infra.Auth/"]
COPY ["VisoERP.Infra.Data/VisoERP.Infra.Data.csproj", "VisoERP.Infra.Data/"]
COPY ["VisoERP.Infra.Helper/VisoERP.Infra.Helper.csproj", "VisoERP.Infra.Helper/"]
COPY ["VisoERP.Infra.Ioc/VisoERP.Infra.Ioc.csproj", "VisoERP.Infra.Ioc/"]
RUN dotnet restore "VisoERP.Web/VisoERP.Web.csproj"

COPY . .
RUN dotnet publish "VisoERP.Web/VisoERP.Web.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
USER root
RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .
USER app

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "VisoERP.Web.dll"]
