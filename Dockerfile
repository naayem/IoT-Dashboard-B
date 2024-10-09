# Étape 1 : Utiliser l'image .NET 8 SDK pour construire l'application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copier les fichiers de solution et de projet
COPY *.sln .
COPY src/IoTDashboard/IoTDashboard.csproj ./src/IoTDashboard/
COPY src/IoTDashboard.Tests/IoTDashboard.Tests.csproj ./src/IoTDashboard.Tests/

# Restaurer les dépendances
RUN dotnet restore

# Copier le reste des fichiers et construire l'application en mode Release
COPY . .
RUN dotnet build --configuration Release --no-restore

# Publier l'application
RUN dotnet publish src/IoTDashboard/IoTDashboard.csproj --configuration Release --output /app/publish

# Étape 2 : Utiliser l'image .NET ASP.NET Core Runtime pour exécuter l'application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Exposer le port 5110 (ou un autre port si nécessaire)
EXPOSE 5110

# Point d'entrée pour exécuter l'application
ENTRYPOINT ["dotnet", "IoTDashboard.dll"]
