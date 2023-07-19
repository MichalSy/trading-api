#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
WORKDIR /src
COPY ["TradingApi/TradingApi.csproj", "TradingApi/"]
RUN dotnet restore "TradingApi/TradingApi.csproj"
COPY . .
WORKDIR "/src/TradingApi"
RUN dotnet build "TradingApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TradingApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TradingApi.dll"]