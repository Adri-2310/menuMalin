# Installation et Configuration

Guide complet pour installer et configurer MenuMalin en environnement développement et production.

## Prérequis

- **Runtime**: .NET 9.0+
- **Database**: MySQL 8.0+
- **Node.js**: 18+ (pour les outils frontend, optionnel)
- **IDE**: Visual Studio 2022 ou VS Code avec C# DevKit

## Installation Développement

### 1. Cloner le Repository

```bash
git clone <repo-url>
cd menuMalin
dotnet restore
```

### 2. Configuration Base de Données

#### Option A: Via Entity Framework (Recommandé)

```bash
# Naviguer au dossier serveur
cd menuMalin.Server

# Appliquer les migrations
dotnet ef database update

# Ou avec une chaîne spécifique
dotnet ef database update --connection "Server=localhost;Port=3306;Database=menuMalin;User Id=root;Password=root;"
```

#### Option B: Création Manuelle

```sql
CREATE DATABASE menuMalin;
USE menuMalin;
-- Les migrations crééront les tables automatiquement
```

### 3. Configuration des Secrets (Production)

Créer `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;Port=3306;Database=menuMalin;User Id=user;Password=SecurePassword123;"
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "EnableSsl": "true",
    "Username": "${SMTP_USERNAME}",
    "Password": "${SMTP_PASSWORD}",
    "FromEmail": "${SMTP_FROM_EMAIL}",
    "ToEmail": "${SMTP_TO_EMAIL}"
  }
}
```

### 4. Variables d'Environnement

Créer un fichier `.env` (non versionné):

```bash
# SMTP Configuration
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-password
SMTP_FROM_EMAIL=noreply@menumain.com
SMTP_TO_EMAIL=admin@menumain.com

# Database (optionnel si dans appsettings)
DB_HOST=localhost
DB_PORT=3306
DB_USER=root
DB_PASSWORD=root

# API Keys
THEMEALDB_API_KEY=optional
```

## Lancement en Développement

### Backend

```bash
cd menuMalin.Server
dotnet run
# Accès: https://localhost:7057
# API: https://localhost:7057/api
# Swagger: https://localhost:7057/openapi/v1.json
```

### Frontend (Blazor WASM)

Le frontend est servi automatiquement avec le backend. Accédez à `https://localhost:7057` dans un navigateur.

### Mode Debug

**Visual Studio:**
- Ouvrir menuMalin.sln
- Sélectionner le profil "https"
- Appuyer sur F5

**VS Code:**
```bash
dotnet run --no-build --configuration Debug
```

## Configuration Avancée

### Kestrel (Port Personnalisé)

Modifier `Properties/launchSettings.json`:

```json
{
  "profiles": {
    "https": {
      "commandName": "Project",
      "applicationUrl": "https://localhost:8443"
    }
  }
}
```

### Logging

Modifier `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.AspNetCore": "Information",
      "System.Net.Http": "Warning",
      "menuMalin.Server": "Information"
    }
  }
}
```

### CORS Configuration

Pour un frontend sur un port différent, modifier `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://your-frontend:port")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

## Tests

### Exécuter les Tests Unitaires

```bash
dotnet test
```

### Test Manual API

```bash
# Connexion
curl -X POST https://localhost:7057/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Password123!"
  }' \
  --insecure

# Récupérer favoris
curl -X GET https://localhost:7057/api/favorites \
  -H "Authorization: Bearer <token>" \
  --insecure
```

## Dépannage Installation

### MySQL Non Accessible

```
⚠️  MySQL pas accessible - migrations ignorées
```

**Solution:**
```bash
# Vérifier le service MySQL
# Linux/Mac: sudo systemctl status mysql
# Windows: services.msc

# Ou exécuter manuellement:
dotnet ef database update
```

### Erreur de Port Utilisé

```
fail: Microsoft.AspNetCore.Server.Kestrel[0] Unable to bind to address https://localhost:7057
```

**Solutions:**
```bash
# Vérifier les ports utilisés
# Windows: netstat -ano | findstr 7057
# Linux/Mac: lsof -i :7057

# Modifier le port dans launchSettings.json
# Ou terminer le processus utilisant le port
```

### Erreur de Connection String

```
Connection string 'DefaultConnection' not found
```

**Solution:**
- Vérifier que `appsettings.Development.json` contient `ConnectionStrings.DefaultConnection`
- Vérifier la syntaxe de la connection string

## Déploiement Production

### Publier l'Application

```bash
dotnet publish -c Release -o ./publish
```

### Hébergement Recommandé

- **Cloud**: Azure App Service, AWS Elastic Beanstalk, DigitalOcean App Platform
- **On-Premise**: IIS (Windows), Docker (Linux)
- **Database**: Azure SQL, AWS RDS, DigitalOcean Managed Databases

### Configuration HTTPS Production

```bash
# Générer un certificat SSL/TLS
dotnet dev-certs https --trust

# Ou utiliser Let's Encrypt via certbot
sudo certbot certonly --standalone -d menumain.com
```

### Docker Deployment

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY ./publish .
EXPOSE 7057
ENV ASPNETCORE_URLS=https://+:7057
ENTRYPOINT ["dotnet", "menuMalin.Server.dll"]
```

```bash
docker build -t menumain:latest .
docker run -p 7057:7057 -e ConnectionStrings__DefaultConnection="..." menumain:latest
```

## Checklist Pré-Production

- [ ] Database sauvegardée
- [ ] Variables d'environnement configurées
- [ ] HTTPS/TLS activé
- [ ] CORS restreint aux domaines autorisés
- [ ] Logging configuré (niveau Warning/Error)
- [ ] Rate limiting implémenté
- [ ] Backups automatiques en place
- [ ] Monitoring/Alertes configurés

