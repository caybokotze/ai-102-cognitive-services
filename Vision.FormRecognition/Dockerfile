FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Vision.FormRecognition/Vision.FormRecognition.csproj", "Vision.FormRecognition/"]
RUN dotnet restore "Vision.FormRecognition/Vision.FormRecognition.csproj"
COPY . .
WORKDIR "/src/Vision.FormRecognition"
RUN dotnet build "Vision.FormRecognition.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vision.FormRecognition.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vision.FormRecognition.dll"]
