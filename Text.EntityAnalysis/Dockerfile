FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Text.SentimentAnalysis/Text.SentimentAnalysis.csproj", "Text.SentimentAnalysis/"]
RUN dotnet restore "Text.SentimentAnalysis/Text.SentimentAnalysis.csproj"
COPY . .
WORKDIR "/src/Text.SentimentAnalysis"
RUN dotnet build "Text.SentimentAnalysis.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Text.SentimentAnalysis.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Text.SentimentAnalysis.dll"]
