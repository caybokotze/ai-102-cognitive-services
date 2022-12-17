FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Vision.ThumbnailGenerator/Vision.ThumbnailGenerator.csproj", "Vision.ThumbnailGenerator/"]
RUN dotnet restore "Vision.ThumbnailGenerator/Vision.ThumbnailGenerator.csproj"
COPY . .
WORKDIR "/src/Vision.ThumbnailGenerator"
RUN dotnet build "Vision.ThumbnailGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vision.ThumbnailGenerator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vision.ThumbnailGenerator.dll"]
