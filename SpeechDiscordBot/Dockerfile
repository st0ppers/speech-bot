#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#WORKDIR /app
#
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["./SpeechDiscordBot.csproj", ""]
#RUN dotnet restore "SpeechDiscordBot.csproj"
#COPY . .
#WORKDIR "/src"
#RUN dotnet build "SpeechDiscordBot.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#ARG OS_CONFIGURATION=win-x64
#RUN dotnet publish "SpeechDiscordBot.csproj" -c $BUILD_CONFIGURATION -r $OS_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "SpeechDiscordBot.dll"]
#

FROM mcr.microsoft.com/dotnet/aspnet:8.0-windowsservercore-ltsc2019 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2019 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./SpeechDiscordBot.csproj", ""]
RUN dotnet restore "SpeechDiscordBot.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "SpeechDiscordBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SpeechDiscordBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM livepeer/ffmpeg-base:windows AS ffmpeg

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpeechDiscordBot.dll"]

