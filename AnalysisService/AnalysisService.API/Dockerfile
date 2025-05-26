FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AnalysisService/AnalysisService.API/ProductReviewAnalyzer.AnalysisService.API.csproj", "AnalysisService/AnalysisService.API/"]
COPY ["AnalysisService/AnalysisService.Application/ProductReviewAnalyzer.AnalysisService.Application.csproj", "AnalysisService/AnalysisService.Application/"]
COPY ["AnalysisService/AnalysisService.Domain/ProductReviewAnalyzer.AnalysisService.Domain.csproj", "AnalysisService/AnalysisService.Domain/"]
COPY ["AnalysisService/AnalysisService.Infrastructure/ProductReviewAnalyzer.AnalysisService.Infrastructure.csproj", "AnalysisService/AnalysisService.Infrastructure/"]
COPY ["AnalysisService/AnalysisService.Messaging/ProductReviewAnalyzer.AnalysisService.Messaging.csproj", "AnalysisService/AnalysisService.Messaging/"]
COPY ["Contracts/ProductReviewAnalyzer.Contracts.csproj", "Contracts/"]
RUN dotnet restore "./AnalysisService/AnalysisService.API/ProductReviewAnalyzer.AnalysisService.API.csproj"
COPY . .
WORKDIR "/src/AnalysisService/AnalysisService.API"
RUN dotnet build "./ProductReviewAnalyzer.AnalysisService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductReviewAnalyzer.AnalysisService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductReviewAnalyzer.AnalysisService.API.dll"]