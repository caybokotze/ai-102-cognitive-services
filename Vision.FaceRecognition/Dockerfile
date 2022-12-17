FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Vision.FaceRecognition/Vision.FaceRecognition.csproj", "Vision.FaceRecognition/"]
RUN dotnet restore "Vision.FaceRecognition/Vision.FaceRecognition.csproj"
COPY . .
WORKDIR "/src/Vision.FaceRecognition"
RUN dotnet build "Vision.FaceRecognition.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vision.FaceRecognition.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vision.FaceRecognition.dll"]
