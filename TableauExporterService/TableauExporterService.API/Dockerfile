FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TableauExporterService/TableauExporterService.API/ProductReviewAnalyzer.TableauExporterService.API.csproj", "TableauExporterService/TableauExporterService.API/"]
COPY ["TableauExporterService/TableauExporterService.Features/ProductReviewAnalyzer.TableauExporterService.Features.csproj", "TableauExporterService/TableauExporterService.Features/"]
COPY ["Contracts/ProductReviewAnalyzer.Contracts.csproj", "Contracts/"]
COPY ["TableauExporterService/TableauExporterService.Infrastructure/ProductReviewAnalyzer.TableauExporterService.Infrastructure.csproj", "TableauExporterService/TableauExporterService.Infrastructure/"]
RUN dotnet restore "./TableauExporterService/TableauExporterService.API/ProductReviewAnalyzer.TableauExporterService.API.csproj"
COPY . .
WORKDIR "/src/TableauExporterService/TableauExporterService.API"
RUN dotnet build "./ProductReviewAnalyzer.TableauExporterService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductReviewAnalyzer.TableauExporterService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductReviewAnalyzer.TableauExporterService.API.dll"]