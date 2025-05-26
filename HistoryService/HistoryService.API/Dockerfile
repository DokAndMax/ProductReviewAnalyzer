FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HistoryService/HistoryService.API/ProductReviewAnalyzer.HistoryService.API.csproj", "HistoryService/HistoryService.API/"]
COPY ["HistoryService/HistoryService.Application/ProductReviewAnalyzer.HistoryService.Application.csproj", "HistoryService/HistoryService.Application/"]
COPY ["HistoryService/HistoryService.Domain/ProductReviewAnalyzer.HistoryService.Domain.csproj", "HistoryService/HistoryService.Domain/"]
COPY ["HistoryService/HistoryService.Infrastructure/ProductReviewAnalyzer.HistoryService.Infrastructure.csproj", "HistoryService/HistoryService.Infrastructure/"]
COPY ["Common/ProductReviewAnalyzer.Common.csproj", "Common/"]
RUN dotnet restore "./HistoryService/HistoryService.API/ProductReviewAnalyzer.HistoryService.API.csproj"
COPY . .
WORKDIR "/src/HistoryService/HistoryService.API"
RUN dotnet build "./ProductReviewAnalyzer.HistoryService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductReviewAnalyzer.HistoryService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductReviewAnalyzer.HistoryService.API.dll"]