FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebApp/ProductReviewAnalyzer.WebApp.csproj", "WebApp/"]
COPY ["AnalysisTrackerService/AnalysisTrackerService.API/ProductReviewAnalyzer.AnalysisTrackerService.API.csproj", "AnalysisTrackerService/AnalysisTrackerService.API/"]
COPY ["AnalysisTrackerService/AnalysisTrackerService.Application/ProductReviewAnalyzer.AnalysisTrackerService.Application.csproj", "AnalysisTrackerService/AnalysisTrackerService.Application/"]
COPY ["AnalysisTrackerService/AnalysisTrackerService.Domain/ProductReviewAnalyzer.AnalysisTrackerService.Domain.csproj", "AnalysisTrackerService/AnalysisTrackerService.Domain/"]
COPY ["AnalysisTrackerService/AnalysisTrackerService.Infrastructure/ProductReviewAnalyzer.AnalysisTrackerService.Infrastructure.csproj", "AnalysisTrackerService/AnalysisTrackerService.Infrastructure/"]
COPY ["Common/ProductReviewAnalyzer.Common.csproj", "Common/"]
COPY ["ReviewsScraper.Rozetka/ProductReviewAnalyzer.ReviewsScraper.Rozetka.csproj", "ReviewsScraper.Rozetka/"]
COPY ["Contracts/ProductReviewAnalyzer.Contracts.csproj", "Contracts/"]
RUN dotnet restore "./WebApp/ProductReviewAnalyzer.WebApp.csproj"
COPY . .
WORKDIR "/src/WebApp"
RUN dotnet build "./ProductReviewAnalyzer.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductReviewAnalyzer.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductReviewAnalyzer.WebApp.dll"]