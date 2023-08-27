FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Ates.Accounting/Ates.Accounting/Ates.Accounting.csproj", "src/Ates.Accounting/Ates.Accounting/"]
RUN dotnet restore "src/Ates.Accounting/Ates.Accounting/Ates.Accounting.csproj"
COPY . .
WORKDIR "/src/src/Ates.Accounting/Ates.Accounting"
RUN dotnet build "Ates.Accounting.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ates.Accounting.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ates.Accounting.dll"]
