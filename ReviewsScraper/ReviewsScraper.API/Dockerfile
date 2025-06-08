FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ReviewsScraper/ReviewsScraper.API/ReviewsScraper.API.csproj", "ReviewsScraper/ReviewsScraper.API/"]
COPY ["ReviewsScraper/ReviewsScraper.Allo/ProductReviewAnalyzer.ReviewsScraper.Allo.csproj", "ReviewsScraper/ReviewsScraper.Allo/"]
COPY ["ReviewsScraper/ReviewsScraper.Core/ProductReviewAnalyzer.ReviewsScraper.Core.csproj", "ReviewsScraper/ReviewsScraper.Core/"]
COPY ["Common/ProductReviewAnalyzer.Common.csproj", "Common/"]
COPY ["Contracts/ProductReviewAnalyzer.Contracts.csproj", "Contracts/"]
COPY ["ReviewsScraper/ReviewsScraper.Foxtrot/ProductReviewAnalyzer.ReviewsScraper.Foxtrot.csproj", "ReviewsScraper/ReviewsScraper.Foxtrot/"]
COPY ["ReviewsScraper/ReviewsScraper.Rozetka/ProductReviewAnalyzer.ReviewsScraper.Rozetka.csproj", "ReviewsScraper/ReviewsScraper.Rozetka/"]
RUN dotnet restore "./ReviewsScraper/ReviewsScraper.API/ReviewsScraper.API.csproj"
COPY . .
WORKDIR "/src/ReviewsScraper/ReviewsScraper.API"
RUN dotnet build "./ReviewsScraper.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ReviewsScraper.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReviewsScraper.API.dll"]