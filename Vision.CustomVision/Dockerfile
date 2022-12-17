FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Vision.CustomVision/Vision.CustomVision.csproj", "Vision.CustomVision/"]
RUN dotnet restore "Vision.CustomVision/Vision.CustomVision.csproj"
COPY . .
WORKDIR "/src/Vision.CustomVision"
RUN dotnet build "Vision.CustomVision.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vision.CustomVision.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vision.CustomVision.dll"]
