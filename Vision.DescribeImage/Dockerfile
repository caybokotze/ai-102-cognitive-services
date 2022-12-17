FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Vision.DescribeImage/Vision.DescribeImage.csproj", "Vision.DescribeImage/"]
RUN dotnet restore "Vision.DescribeImage/Vision.DescribeImage.csproj"
COPY . .
WORKDIR "/src/Vision.DescribeImage"
RUN dotnet build "Vision.DescribeImage.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vision.DescribeImage.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vision.DescribeImage.dll"]
