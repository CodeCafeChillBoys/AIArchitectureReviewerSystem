FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["DiagramManagerService/DiagramManager.API/DiagramManager.API.csproj", "DiagramManagerService/DiagramManager.API/"]
COPY ["DiagramManagerService/DiagramManager.Application/DiagramManager.Application.csproj", "DiagramManagerService/DiagramManager.Application/"]
COPY ["DiagramManagerService/DiagramManager.Domain/DiagramManager.Domain.csproj", "DiagramManagerService/DiagramManager.Domain/"]
COPY ["DiagramManagerService/DiagramManager.Infrastructure/DiagramManager.Infrastructure.csproj", "DiagramManagerService/DiagramManager.Infrastructure/"]
COPY ["DiagramManagerService/Shared.Messaging/Shared.Messaging.csproj", "DiagramManagerService/Shared.Messaging/"]
RUN dotnet restore "DiagramManagerService/DiagramManager.API/DiagramManager.API.csproj"
COPY . .
WORKDIR "/src/DiagramManagerService/DiagramManager.API"
RUN dotnet publish "DiagramManager.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "DiagramManager.API.dll"]
