FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Text.AnsweringQuestions/Text.AnsweringQuestions.csproj", "Text.AnsweringQuestions/"]
RUN dotnet restore "Text.AnsweringQuestions/Text.AnsweringQuestions.csproj"
COPY . .
WORKDIR "/src/Text.AnsweringQuestions"
RUN dotnet build "Text.AnsweringQuestions.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Text.AnsweringQuestions.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Text.AnsweringQuestions.dll"]
