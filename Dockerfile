#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Vocab.WebApi/Vocab.WebApi.csproj", "src/Vocab.WebApi/"]
COPY ["src/Vocab.Application/Vocab.Application.csproj", "src/Vocab.Application/"]
COPY ["src/Vocab.Core/Vocab.Core.csproj", "src/Vocab.Core/"]
COPY ["src/Vocab.Infrastructure/Vocab.Infrastructure.csproj", "src/Vocab.Infrastructure/"]
RUN dotnet restore "./src/Vocab.WebApi/Vocab.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Vocab.WebApi"
RUN dotnet build "./Vocab.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Vocab.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ConnectionStrings__PostgreSql=""
ENV ConnectionStrings__HangfireConnection=""
ENV Keycloak__auth-server-url=""
ENV Keycloak__realm="vocab"
ENV Keycloak__resource="aspnet"
ENV Keycloak__credentials__secret="secret"
ENV Cors__Origins=""

ENTRYPOINT ["dotnet", "Vocab.WebApi.dll"]