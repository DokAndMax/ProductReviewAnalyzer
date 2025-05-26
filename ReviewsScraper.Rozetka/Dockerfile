FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ReviewsScraper.Rozetka/ProductReviewAnalyzer.ReviewsScraper.Rozetka.csproj", "ReviewsScraper.Rozetka/"]
COPY ["Common/ProductReviewAnalyzer.Common.csproj", "Common/"]
COPY ["Contracts/ProductReviewAnalyzer.Contracts.csproj", "Contracts/"]
RUN dotnet restore "./ReviewsScraper.Rozetka/ProductReviewAnalyzer.ReviewsScraper.Rozetka.csproj"
COPY . .
WORKDIR "/src/ReviewsScraper.Rozetka"
RUN dotnet build "./ProductReviewAnalyzer.ReviewsScraper.Rozetka.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductReviewAnalyzer.ReviewsScraper.Rozetka.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductReviewAnalyzer.ReviewsScraper.Rozetka.dll"]