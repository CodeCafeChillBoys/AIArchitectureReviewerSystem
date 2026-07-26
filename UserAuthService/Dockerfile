FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["UserAuthService/UserAuthService.API/UserAuthService.API.csproj", "UserAuthService/UserAuthService.API/"]
COPY ["UserAuthService/UserAuthService.Application/UserAuthService.Application.csproj", "UserAuthService/UserAuthService.Application/"]
COPY ["UserAuthService/UserAuthService.Domain/UserAuthService.Domain.csproj", "UserAuthService/UserAuthService.Domain/"]
COPY ["UserAuthService/UserAuthService.Infrastructure/UserAuthService.Infrastructure.csproj", "UserAuthService/UserAuthService.Infrastructure/"]
RUN dotnet restore "UserAuthService/UserAuthService.API/UserAuthService.API.csproj"
COPY . .
WORKDIR "/src/UserAuthService/UserAuthService.API"
RUN dotnet publish "UserAuthService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "UserAuthService.API.dll"]
