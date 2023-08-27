FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Ates.Auth/Ates.Auth.csproj", "src/Ates.Auth/"]
RUN dotnet restore "src/Ates.Auth/Ates.Auth.csproj"
COPY . .
WORKDIR "/src/src/Ates.Auth"
RUN dotnet build "Ates.Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ates.Auth.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ates.Auth.dll"]
