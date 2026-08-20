FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["AIArchitectureReviewer/AIArchitectureReviewer.API/AIArchitectureReviewer.API.csproj", "AIArchitectureReviewer/AIArchitectureReviewer.API/"]
COPY ["AIArchitectureReviewer/AIArchitectureReviewer.Application/AIArchitectureReviewer.Application.csproj", "AIArchitectureReviewer/AIArchitectureReviewer.Application/"]
COPY ["AIArchitectureReviewer/AIArchitectureReviewer.Infrastructure/AIArchitectureReviewer.Infrastructure.csproj", "AIArchitectureReviewer/AIArchitectureReviewer.Infrastructure/"]
RUN dotnet restore "AIArchitectureReviewer/AIArchitectureReviewer.API/AIArchitectureReviewer.API.csproj"
COPY . .
WORKDIR "/src/AIArchitectureReviewer/AIArchitectureReviewer.API"
RUN dotnet publish "AIArchitectureReviewer.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "AIArchitectureReviewer.API.dll"]
