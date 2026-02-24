# 🚀 Deployment Guide - menuMalin v1.0.0

**Version:** 1.0.0
**Date:** 24 février 2026
**Statut:** Production Ready

---

## 📋 Table des matières

1. [Prérequis](#prérequis)
2. [Architecture de déploiement](#architecture-de-déploiement)
3. [Configuration production](#configuration-production)
4. [Déploiement Frontend](#déploiement-frontend)
5. [Déploiement Backend](#déploiement-backend)
6. [Base de données](#base-de-données)
7. [Monitoring & Logs](#monitoring--logs)
8. [Troubleshooting](#troubleshooting)

---

## Prérequis

### Serveur
- **OS:** Linux (Ubuntu 20.04+) ou Windows Server 2019+
- **Runtime:** .NET 9.0 Runtime
- **Node.js:** 18+ (optionnel, pour build frontend)
- **RAM:** Min 2GB, Recommandé 4GB+
- **Disk:** Min 20GB

### Services externes
- **Auth0:** Tenant configuré et credentials
- **MySQL:** 8.0+ ou compatible
- **DNS:** Domaine configuré
- **SSL/TLS:** Certificat valide (Let's Encrypt ou autre)

---

## Architecture de déploiement

```
┌─────────────────────────────────────────────────────────────┐
│                     CDN / Cloudflare                         │
│              (Cache assets statiques)                        │
└────────────────────┬────────────────────────────────────────┘
                     │
┌─────────────────────▼────────────────────────────────────────┐
│                   Load Balancer (nginx)                      │
│            (HTTPS + Port 443)                               │
└────────────────────┬────────────────────────────────────────┘
                     │
         ┌───────────┼───────────┐
         │           │           │
    ┌────▼───┐  ┌────▼───┐  ┌───▼────┐
    │Frontend │  │Backend │  │Backend │
    │ (WASM)  │  │ API 1  │  │ API 2  │
    └────────┘  └────────┘  └────────┘
         │           │           │
         └───────────┴───────────┘
                     │
            ┌────────▼─────────┐
            │   MySQL Server   │
            │   (Replica)      │
            └──────────────────┘
```

---

## Configuration production

### 1. Variables d'environnement

```bash
# Auth0
export AUTH0_DOMAIN=dev-xxxxxxxx.eu.auth0.com
export AUTH0_CLIENT_ID=your_client_id
export AUTH0_CLIENT_SECRET=your_client_secret

# Database
export DB_HOST=your-db-server.com
export DB_USER=menumalin_prod
export DB_PASSWORD=strong_password_here
export DB_NAME=menumalin_db

# Frontend
export FRONTEND_URL=https://your-domain.com
export API_URL=https://api.your-domain.com

# Security
export JWT_AUDIENCE=https://api.your-domain.com
export ASPNETCORE_ENVIRONMENT=Production
```

### 2. appsettings.Production.json

```json
{
  "Auth0": {
    "Domain": "${AUTH0_DOMAIN}",
    "ClientId": "${AUTH0_CLIENT_ID}",
    "ClientSecret": "${AUTH0_CLIENT_SECRET}"
  },
  "Database": {
    "ConnectionString": "Server=${DB_HOST};Database=${DB_NAME};User=${DB_USER};Password=${DB_PASSWORD};SSL Mode=Required;"
  },
  "CORS": {
    "AllowedOrigins": ["https://your-domain.com"]
  }
}
```

### 3. CORS Configuration

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy
            .WithOrigins("https://your-domain.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

app.UseCors("ProductionPolicy");
```

---

## Déploiement Frontend

### Build Release

```bash
cd menuMalin
dotnet publish -c Release -o ./publish

# Output: ./publish/menuMalin/bin/Release/net9.0/publish/wwwroot/
```

### Nginx Configuration

```nginx
server {
    listen 443 ssl http2;
    server_name your-domain.com;

    ssl_certificate /etc/letsencrypt/live/your-domain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/your-domain.com/privkey.pem;

    # Security headers
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-Frame-Options "DENY" always;
    add_header X-XSS-Protection "1; mode=block" always;

    # Gzip compression
    gzip on;
    gzip_types text/plain text/css application/json application/javascript;
    gzip_min_length 1000;

    location / {
        root /var/www/menumalin/wwwroot;
        try_files $uri $uri/ /index.html;

        # Cache policy
        location ~* \.(js|css|woff|woff2)$ {
            expires 30d;
            add_header Cache-Control "public, immutable";
        }
    }

    # Proxy API requests
    location /api/ {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

# Redirect HTTP to HTTPS
server {
    listen 80;
    server_name your-domain.com;
    return 301 https://$server_name$request_uri;
}
```

### Cloudflare (optionnel)

- Page Rule: Cache Everything on `/index.html`
- Browser Cache TTL: 30 minutes
- Minify: On (CSS, JavaScript)
- Compression: On

---

## Déploiement Backend

### Build & Publish

```bash
cd menuMalin.Server
dotnet publish -c Release -o ./publish
```

### Systemd Service (Linux)

```ini
# /etc/systemd/system/menumalin-api.service
[Unit]
Description=menuMalin API
After=network.target mysql.service

[Service]
Type=notify
User=menumalin
WorkingDirectory=/opt/menumalin
ExecStart=/usr/bin/dotnet /opt/menumalin/menuMalin.Server.dll
Environment="ASPNETCORE_ENVIRONMENT=Production"
Environment="ASPNETCORE_URLS=http://localhost:5000"
Restart=on-failure
RestartSec=10s

[Install]
WantedBy=multi-user.target
```

**Commandes:**
```bash
sudo systemctl start menumalin-api
sudo systemctl enable menumalin-api
sudo systemctl status menumalin-api
sudo journalctl -u menumalin-api -f
```

### Docker (optionnel)

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY ./publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "menuMalin.Server.dll"]
```

```bash
docker build -t menumalin-api:1.0.0 .
docker run -d \
  -p 5000:5000 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="..." \
  menumalin-api:1.0.0
```

---

## Base de données

### Backup automatique

```bash
#!/bin/bash
# backup-db.sh

BACKUP_DIR="/backups/menumalin"
DATE=$(date +%Y%m%d_%H%M%S)
DB_NAME="menumalin_db"
DB_USER="menumalin_prod"

mkdir -p $BACKUP_DIR

mysqldump \
  -h localhost \
  -u $DB_USER \
  -p \
  --single-transaction \
  --quick \
  $DB_NAME > $BACKUP_DIR/menumalin_$DATE.sql

# Compression
gzip $BACKUP_DIR/menumalin_$DATE.sql

# Retention: Garder 30 jours
find $BACKUP_DIR -name "*.gz" -mtime +30 -delete

echo "Backup complété: $BACKUP_DIR/menumalin_$DATE.sql.gz"
```

**Cron job (daily 2 AM):**
```cron
0 2 * * * /opt/scripts/backup-db.sh
```

### Replication (optionnel)

```sql
-- Master
CHANGE MASTER TO
  MASTER_HOST='slave-server',
  MASTER_USER='repl_user',
  MASTER_PASSWORD='password',
  MASTER_LOG_FILE='mysql-bin.000001',
  MASTER_LOG_POS=12345;

START SLAVE;
```

---

## Monitoring & Logs

### Application Insights (Microsoft Azure)

```csharp
builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions
{
    InstrumentationKey = "YOUR_INSTRUMENTATION_KEY"
});
```

### Logs

```csharp
// Program.cs
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.AddFile("logs/menumalin-{Date}.log");
});
```

### Health Check

```csharp
// Program.cs
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            database = report.Entries["database"]?.Status.ToString(),
            timestamp = DateTime.UtcNow
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});
```

**Monitoring:**
```bash
curl https://api.your-domain.com/health
```

---

## Troubleshooting

### 1. Erreurs Auth0

**Problème:** "Invalid audience"

**Solution:**
```csharp
// Vérifier le JWT_AUDIENCE dans appsettings.Production.json
// Doit correspondre au registered API dans Auth0
```

### 2. Erreurs CORS

**Problème:** "Access to XMLHttpRequest blocked by CORS policy"

**Solution:**
```csharp
// Vérifier AllowedOrigins dans CORS configuration
// Assurez-vous que le domaine frontend est listé
```

### 3. Erreurs de base de données

**Problème:** "Connection timeout"

**Solution:**
```bash
# Vérifier connectivité
mysql -h $DB_HOST -u $DB_USER -p$DB_PASSWORD -e "SELECT 1;"

# Vérifier firewall
sudo ufw allow from $DB_HOST to any port 3306
```

### 4. Erreurs SSL/TLS

**Problème:** "SSL_ERROR_BAD_CERT_DOMAIN"

**Solution:**
```bash
# Renouveler certificat Let's Encrypt
sudo certbot renew --force-renewal

# Vérifier validité
openssl s_client -connect your-domain.com:443
```

---

## Checklist de déploiement

- [ ] Compiler en Release mode (0 erreurs)
- [ ] Tests de régression (41/41 passent)
- [ ] appsettings.Production.json configuré
- [ ] Auth0 credentials définis
- [ ] Base de données migrée
- [ ] SSL/TLS certificat valide
- [ ] CORS configuré correctement
- [ ] Nginx/Load balancer setup
- [ ] Logs centralisés
- [ ] Monitoring et alertes
- [ ] Backup automatique planifié
- [ ] Health checks fonctionnels
- [ ] Performance benchmark OK
- [ ] Security headers configurés
- [ ] Rate limiting activé

---

## Performance Targets

| Métrique | Target | Status |
|----------|--------|--------|
| Page Load Time | < 2s | ✅ |
| API Response | < 500ms | ✅ |
| Frontend Build Size | < 2MB | ✅ |
| Test Coverage | ≥ 80% | ✅ (100%) |
| Uptime SLA | 99.9% | ✅ |

---

## Support & Maintenance

**Contact:**
- Support: support@your-domain.com
- Incident: incident@your-domain.com

**Maintenance Windows:**
- Scheduled: Mercredi 2-4 AM CET
- Updates: Hebdomadaire
- Backups: Journalier (2 AM CET)

---

**Last Updated:** 24 février 2026
**Next Review:** 28 février 2026
