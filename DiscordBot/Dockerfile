FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY DiscordBot/*.csproj ./
RUN dotnet restore
COPY DiscordBot/. .
RUN dotnet publish -c Release -o publish

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DiscordBot.dll"]