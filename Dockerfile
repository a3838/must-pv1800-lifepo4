FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["must-pv1800.csproj", "./"]
RUN dotnet restore "must-pv1800.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "must-pv1800.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "must-pv1800.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "must-pv1800.dll"]
