FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Text.Translate/Text.Translate.csproj", "Text.Translate/"]
RUN dotnet restore "Text.Translate/Text.Translate.csproj"
COPY . .
WORKDIR "/src/Text.Translate"
RUN dotnet build "Text.Translate.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Text.Translate.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Text.Translate.dll"]
