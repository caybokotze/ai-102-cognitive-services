FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Vision.VideoIndexer/Vision.VideoIndexer.csproj", "Vision.VideoIndexer/"]
RUN dotnet restore "Vision.VideoIndexer/Vision.VideoIndexer.csproj"
COPY . .
WORKDIR "/src/Vision.VideoIndexer"
RUN dotnet build "Vision.VideoIndexer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vision.VideoIndexer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vision.VideoIndexer.dll"]
