#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Script de test pour diagnostiquer les problèmes d'authentification JWT avec Auth0

.DESCRIPTION
    Ce script teste les endpoints du backend pour vérifier que la configuration JWT fonctionne correctement.

.EXAMPLE
    .\test-auth0-jwt.ps1
#>

param(
    [string]$BackendUrl = "http://localhost:5266",
    [string]$Token = ""
)

# Couleurs pour l'affichage
$Colors = @{
    Success = "Green"
    Error = "Red"
    Warning = "Yellow"
    Info = "Cyan"
}

function Write-Status {
    param(
        [string]$Message,
        [string]$Status = "Info"
    )
    $Color = $Colors[$Status]
    Write-Host "[$Status] $Message" -ForegroundColor $Color
}

Write-Host "`n=== Test d'Authentification Auth0 JWT ===" -ForegroundColor Cyan
Write-Host "Backend URL: $BackendUrl`n"

# Test 1: Vérifier que le backend est accessible
Write-Status "Test 1: Vérification de la connexion au backend" -Status "Info"
try {
    $healthResponse = Invoke-WebRequest -Uri "$BackendUrl/api/tokendebug/health" -ErrorAction Stop
    Write-Status "Backend accessible" -Status "Success"
    Write-Host "Réponse: $($healthResponse.Content)" -ForegroundColor Green
} catch {
    Write-Status "Impossible de se connecter au backend: $_" -Status "Error"
    Write-Host "Assurez-vous que le backend tourne sur $BackendUrl" -ForegroundColor Yellow
    exit 1
}

# Test 2: Vérifier la configuration attendue
Write-Status "`nTest 2: Vérification de la configuration Auth0" -Status "Info"
try {
    $configResponse = Invoke-WebRequest -Uri "$BackendUrl/api/tokendebug/expected-config"
    $configJson = $configResponse.Content | ConvertFrom-Json
    Write-Host "Configuration attendue:" -ForegroundColor Cyan
    Write-Host "  - Audience: $($configJson.expectedAudience)"
    Write-Host "  - Domain: $($configJson.expectedDomain)"
    Write-Host "  - Authority: $($configJson.expectedAuthority)"
    Write-Status "Configuration OK" -Status "Success"
} catch {
    Write-Status "Erreur lors de la lecture de la configuration: $_" -Status "Error"
}

# Test 3: Si un token est fourni, le tester
if (-not [string]::IsNullOrEmpty($Token)) {
    Write-Status "`nTest 3: Vérification du token JWT" -Status "Info"

    try {
        $headers = @{
            "Authorization" = "Bearer $Token"
            "Content-Type" = "application/json"
        }

        $tokenResponse = Invoke-WebRequest -Uri "$BackendUrl/api/tokendebug/token-info" `
                                          -Headers $headers `
                                          -ErrorAction Stop

        $tokenJson = $tokenResponse.Content | ConvertFrom-Json

        Write-Host "✅ Token valide et authentification réussie!" -ForegroundColor Green
        Write-Host "`nInformations du token:" -ForegroundColor Cyan
        Write-Host "  - Authentifié: $($tokenJson.isAuthenticated)"
        Write-Host "  - Identity: $($tokenJson.identity)"
        Write-Host "  - Expire: $($tokenJson.tokenExpiresAt)"
        Write-Host "  - Expiré: $($tokenJson.isExpired)"

        Write-Host "`nClaims du token:" -ForegroundColor Cyan
        foreach ($claim in $tokenJson.claims.GetEnumerator()) {
            Write-Host "  - $($claim.Key): $($claim.Value)"
        }

    } catch {
        if ($_.Exception.Response.StatusCode -eq 401) {
            Write-Status "❌ Token rejeté (401 Unauthorized)" -Status "Error"
            Write-Host "Cela signifie que le token n'a pas passé la validation JWT." -ForegroundColor Yellow
            Write-Host "Points à vérifier:" -ForegroundColor Yellow
            Write-Host "  1. L'Audience du token correspond-elle à '$($configJson.expectedAudience)'?"
            Write-Host "  2. Le token est-il expiré?"
            Write-Host "  3. Le domain Auth0 est-il correct?"
        } else {
            Write-Status "Erreur lors du test du token: $_" -Status "Error"
        }
    }
} else {
    Write-Status "`nTest 3: Token JWT - Skippé" -Status "Warning"
    Write-Host "Pour tester avec un token, utilisez: " -NoNewline
    Write-Host ".\test-auth0-jwt.ps1 -Token 'YOUR_TOKEN_HERE'" -ForegroundColor Cyan
    Write-Host "`nObtenir un token:" -ForegroundColor Yellow
    Write-Host "  1. Connectez-vous au frontend (https://localhost:7777)"
    Write-Host "  2. Ouvrez la console du navigateur (F12)"
    Write-Host "  3. Dans l'onglet 'Application' ou 'Storage', allez à 'Local Storage'"
    Write-Host "  4. Cherchez la clé contenant votre token (généralement 'auth...')"
    Write-Host "  5. Copiez la valeur du token"
}

# Test 4: Obtenir les infos utilisateur
if (-not [string]::IsNullOrEmpty($Token)) {
    Write-Status "`nTest 4: Récupération des infos utilisateur" -Status "Info"

    try {
        $headers = @{
            "Authorization" = "Bearer $Token"
            "Content-Type" = "application/json"
        }

        $userResponse = Invoke-WebRequest -Uri "$BackendUrl/api/tokendebug/current-user" `
                                         -Headers $headers `
                                         -ErrorAction Stop

        $userJson = $userResponse.Content | ConvertFrom-Json

        Write-Host "Infos utilisateur:" -ForegroundColor Green
        Write-Host "  - ID: $($userJson.userId)"
        Write-Host "  - Email: $($userJson.email)"
        Write-Host "  - Name: $($userJson.name)"
        Write-Host "  - Sub: $($userJson.sub)"
        Write-Host "  - Audience: $($userJson.audience ?? 'ABSENT - Problème!')"
        Write-Host "  - Scopes: $($userJson.scopes ?? 'Aucun')"

        Write-Status "Utilisateur récupéré avec succès" -Status "Success"

    } catch {
        Write-Status "Erreur lors de la récupération des infos utilisateur: $_" -Status "Error"
    }
}

Write-Host "`n=== Fin des tests ===" -ForegroundColor Cyan
