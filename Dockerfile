FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY PlataformaAutogestion.Domain/PlataformaAutogestion.Domain.csproj PlataformaAutogestion.Domain/
COPY PlataformaAutogestion.Application/PlataformaAutogestion.Application.csproj PlataformaAutogestion.Application/
COPY PlataformaAutogestion.Infrastructure/PlataformaAutogestion.Infrastructure.csproj PlataformaAutogestion.Infrastructure/
COPY PlataformaAutogestion.Api/PlataformaAutogestion.Api.csproj PlataformaAutogestion.Api/
RUN dotnet restore PlataformaAutogestion.Api/PlataformaAutogestion.Api.csproj

COPY . .
RUN dotnet publish PlataformaAutogestion.Api/PlataformaAutogestion.Api.csproj \
    --configuration Release --no-restore --output /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Fuentes para los recibos generados con QuestPDF en Linux.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libfontconfig1 fonts-dejavu-core \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000
USER $APP_UID
CMD ["sh", "-c", "exec dotnet PlataformaAutogestion.Api.dll --urls http://0.0.0.0:${PORT:-10000}"]
