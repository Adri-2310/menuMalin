# Guide Complet Auth0 pour Blazor + ASP.NET Core

## Table des matières
1. [Introduction à Auth0](#introduction-à-auth0)
2. [Créer un compte Auth0](#créer-un-compte-auth0)
3. [Créer une application dans Auth0](#créer-une-application-dans-auth0)
4. [Configurer les URLs](#configurer-les-urls)
5. [Récupérer les credentials](#récupérer-les-credentials)
6. [Créer une API dans Auth0](#créer-une-api-dans-auth0)
7. [Configurer les rôles et permissions](#configurer-les-rôles-et-permissions)
8. [Flow de login/logout](#flow-de-loginlogout)
9. [Stockage des credentials](#stockage-des-credentials)
10. [Testing du login](#testing-du-login)

---

## 1. Introduction à Auth0

### Qu'est-ce que Auth0?

Auth0 est une plateforme d'authentification et d'autorisation basée sur le cloud (Identity Platform as a Service - IDaaS). Elle fournit une solution complète pour gérer l'authentification et l'autorisation des utilisateurs sans avoir à construire et maintenir votre propre système d'authentification.

### Fonctionnalités principales:
- Authentification multi-protocoles (OAuth 2.0, OpenID Connect, SAML)
- Gestion des utilisateurs et des identités
- Support de multiples fournisseurs d'identité (Google, GitHub, Microsoft, Facebook, etc.)
- Authentification multi-facteur (MFA)
- Gestion des rôles et permissions
- Logs et analytics
- Règles et actions personnalisées
- Dashboard d'administration

### Pourquoi utiliser Auth0?

**Sécurité:**
- Infrastructure sécurisée et conforme aux normes (GDPR, SOC 2, ISO 27001)
- Gestion automatique des tokens et des secrets
- Protection contre les attaques courantes (injection, CSRF, XSS)
- Chiffrement des données sensibles

**Scalabilité:**
- Infrastructure cloud gérée
- Pas besoin de gérer les serveurs d'authentification
- Support automatique du scaling
- Disponibilité mondiale avec CDN

**Flexibilité:**
- Intégration avec n'importe quelle plateforme (web, mobile, desktop)
- Support de multiples protocoles d'authentification
- Personnalisation complète du flux de login
- Extensibilité via des règles et des actions

**Efficacité:**
- Réduit le temps de développement
- Pas de maintenance de l'infrastructure d'authentification
- Réduction des bugs liés à la sécurité
- Focus sur la logique métier plutôt que l'auth

**Expérience utilisateur:**
- Authentification sociale (login avec Google, GitHub, etc.)
- Authentification sans mot de passe (passwordless)
- Single Sign-On (SSO)
- Expérience de login cohérente

---

## 2. Créer un compte Auth0

### Étapes pour créer un compte:

1. **Accéder au site Auth0**
   - Allez sur https://auth0.com
   - Cliquez sur le bouton "Sign Up" (en haut à droite)

2. **Choisir la méthode d'inscription**
   - Email et mot de passe
   - GitHub
   - Google
   - Microsoft

3. **Remplir le formulaire d'inscription**
   - Prénom et nom
   - Email
   - Mot de passe (si vous choisissez cette option)
   - Accepter les conditions d'utilisation

4. **Vérifier votre email**
   - Auth0 vous enverra un email de confirmation
   - Cliquez sur le lien de confirmation
   - Vous serez redirigé vers le dashboard

5. **Informations sur le compte**
   - Créer ou sélectionner un tenant
   - Sélectionner votre région (Europe, USA, etc.)
   - Accepter les derniers termes

### Après l'inscription:

Vous arriverez sur le **Auth0 Dashboard** qui contient:
- Vue d'ensemble de votre tenant
- Applications
- API
- Utilisateurs
- Logs
- Paramètres du tenant
- Configuration des connexions (Identity Providers)

---

## 3. Créer une application dans Auth0

### Type d'application: Regular Web Application

Pour une application Blazor + ASP.NET Core, nous utilisons le type **Regular Web Application** car:
- L'application web a un backend sécurisé
- Le backend peut stocker des secrets clients
- Nous utilisons le flow Authorization Code with PKCE
- Nous avons besoin de gérer les sessions côté serveur

### Étapes de création:

1. **Naviguer vers Applications**
   - Dans le dashboard Auth0, cliquez sur "Applications" dans le menu de gauche
   - Cliquez sur le bouton "Create Application"

2. **Nommer l'application**
   - Entrez un nom: par exemple "Mon App Blazor" ou "MyBlazorApp"
   - Choisissez le type: **Regular Web Application**
   - Cliquez sur "Create"

3. **Sélectionner le framework**
   - Auth0 vous propose de sélectionner votre framework
   - Pour ASP.NET Core avec Blazor, choisissez **ASP.NET Core**
   - Cliquez sur "Create" (ou "Next" si une autre étape apparaît)

4. **Onglets disponibles après création:**
   - **Settings**: Configuration générale (Domain, Client ID, Credentials, etc.)
   - **Connections**: Connecteurs d'identité
   - **Credentials**: Secrets et métadonnées
   - **API**: Accès aux API
   - **Logs**: Historique des login
   - **Test**: Tester le login

---

## 4. Configurer les URLs

### URLs à configurer dans les settings de l'application:

#### 4.1 Allowed Callback URLs
**Qu'est-ce que c'est?**
Les URLs de rappel (callback) sont les adresses vers lesquelles Auth0 redirigera l'utilisateur après une authentification réussie. L'utilisateur recevra un code d'autorisation ou un token qui sera utilisé pour établir la session.

**URLs pour le développement local:**
```
http://localhost:7000/signin-auth0
http://localhost:5000/signin-auth0
```

**URLs pour la production:**
```
https://monapp.com/signin-auth0
https://www.monapp.com/signin-auth0
```

**Notes:**
- L'URL doit exactement correspondre à celle dans votre application
- Le port doit être correct
- Le protocole (http/https) doit correspondre
- Vous pouvez ajouter plusieurs URLs

#### 4.2 Allowed Logout URLs
**Qu'est-ce que c'est?**
Les URLs de déconnexion sont les adresses vers lesquelles Auth0 redirigera l'utilisateur après une déconnexion.

**URLs pour le développement local:**
```
http://localhost:7000
http://localhost:7000/
http://localhost:5000
http://localhost:5000/
```

**URLs pour la production:**
```
https://monapp.com
https://monapp.com/
https://www.monapp.com
https://www.monapp.com/
```

**Notes:**
- Généralement, c'est votre page d'accueil
- Vous pouvez rediriger vers une page de confirmation de déconnexion

#### 4.3 CORS - Allowed Web Origins
**Qu'est-ce que c'est?**
CORS (Cross-Origin Resource Sharing) configure les domaines qui peuvent faire des requêtes au serveur Auth0 directement (depuis le navigateur). C'est utilisé par la librairie d'Auth0 pour les vérifications.

**URLs pour le développement local:**
```
http://localhost:7000
http://localhost:5000
```

**URLs pour la production:**
```
https://monapp.com
https://www.monapp.com
```

**Notes:**
- Sans le port, si vous avez un port différent
- Le port DOIT être inclus pour le localhost
- L'ordre http/https doit correspondre

#### 4.4 Application URLs
**Qu'est-ce que c'est?**
L'URL de l'application est utilisée pour certaines configurations et communications.

**Pour le développement:**
```
http://localhost:7000
```

**Pour la production:**
```
https://monapp.com
```

### Comment ajouter les URLs:

1. Allez dans l'onglet **Settings** de votre application
2. Cherchez la section "Application URIs"
3. Pour chaque champ (Callback, Logout, CORS, etc.):
   - Cliquez sur le champ d'entrée
   - Entrez l'URL (une par ligne pour plusieurs URLs)
   - Cliquez en dehors du champ ou appuyez sur Entrée
4. Cliquez sur **Save Changes** en bas de page

---

## 5. Récupérer les credentials

### Credentials nécessaires:

Les credentials sont les informations dont vous avez besoin pour configurer votre application. Vous les trouverez dans l'onglet **Settings** de votre application Auth0.

#### 5.1 Domain (ou Tenant)
**Qu'est-ce que c'est?**
Le domaine de votre tenant Auth0. C'est le serveur que votre application contactera pour l'authentification.

**Format:**
```
votrenom.auth0.com
votrenom.eu.auth0.com
votrenom.au.auth0.com
```

**Où le trouver:**
- Dans les settings de l'application: champ "Domain"
- Vous pouvez aussi le voir en haut du dashboard Auth0

**Exemple:**
```
mycompany.auth0.com
```

#### 5.2 Client ID
**Qu'est-ce que c'est?**
L'identifiant unique de votre application. C'est un code alphanumérique qui identifie votre application auprès d'Auth0.

**Format:**
Chaîne alphanumérique d'environ 32 caractères

**Exemple:**
```
a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6
```

**Où le trouver:**
- Dans les settings de l'application: champ "Client ID"
- Cet ID est public et peut être exposé dans le code

#### 5.3 Client Secret
**Qu'est-ce que c'est?**
Un secret cryptographique qui prouve que votre application est autorisée à communiquer avec Auth0. DOIT rester secret.

**Format:**
Chaîne alphanumérique longue (caractères spéciaux inclus)

**Exemple:**
```
a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6_abc123XYZ_def456UVW
```

**Où le trouver:**
- Dans l'onglet **Credentials** (ou parfois dans Settings) de l'application
- Cliquez sur "Show" pour afficher le secret
- Le secret est masqué par défaut par raison de sécurité

**IMPORTANT:**
- Ne jamais commiter ce secret dans Git
- Ne jamais l'exposer sur le web
- Le traiter comme un mot de passe

#### 5.4 Autres informations utiles:

**Application Type:**
```
Regular Web Application
```

**Application URL:**
Votre URL d'application (pour la production)

**Token Endpoint:**
```
https://votrenom.auth0.com/oauth/token
```

**Authorization Endpoint:**
```
https://votrenom.auth0.com/authorize
```

### Copier et stocker les credentials:

1. Ouvrez l'onglet **Settings** de votre application
2. Notez le **Domain**
3. Notez le **Client ID**
4. Cliquez sur l'onglet **Credentials**
5. Cliquez sur "Show" pour afficher le **Client Secret**
6. Copiez-le avec soin (incluez tous les caractères)
7. Stockez-les de manière sécurisée (voir section "Stockage des credentials")

---

## 6. Créer une API dans Auth0

### Qu'est-ce qu'une API dans Auth0?

Une API dans Auth0 représente votre API backend (ASP.NET Core). Elle définit:
- L'identifiant unique de votre API
- Les scopes disponibles (permissions granulaires)
- Les paramètres de sécurité

### Pourquoi créer une API?

- Définir les permissions (scopes) que votre application peut demander
- Valider les tokens JWT émis par Auth0
- Configurer les paramètres de sécurité de l'API
- Gérer les accès à votre API

### Étapes de création:

1. **Naviguer vers APIs**
   - Dans le dashboard Auth0, cliquez sur "Applications" > "APIs" dans le menu de gauche
   - Cliquez sur le bouton "Create API"

2. **Remplir les informations de l'API**
   - **Name**: Nom de votre API (par exemple "Mon API Blazor" ou "MyBlazorBackend")
   - **Identifier**: Identifiant unique (par exemple "https://monapp.com/api" ou "monapp-api")
     - Ceci est important car c'est ce que vous utiliserez pour valider les tokens
     - Format recommandé: "https://domaine.com/api" ou un simple identifiant
   - **Signing Algorithm**: RS256 (par défaut, c'est le bon choix)

3. **Cliquer sur "Create"**

### Après création:

Vous verrez les onglets:
- **Quick Start**: Documentation rapide
- **Settings**: Configuration générale
- **Scopes**: Définir les permissions (voir section suivante)
- **Permissions**: Permissions par application
- **Signing Keys**: Clés de signature des tokens

---

## 7. Configurer les rôles et permissions

### Qu'est-ce que les permissions et les rôles?

**Permissions (Scopes):**
- Actions granulaires que l'utilisateur peut faire
- Exemples: "read:documents", "write:posts", "delete:users"
- Associées aux APIs
- Incluses dans les tokens d'accès

**Rôles:**
- Groupements de permissions
- Exemples: "Admin", "Editor", "Viewer"
- Associés aux utilisateurs
- Contiennent plusieurs permissions

### Flow recommandé:

Pour une application Blazor + ASP.NET Core:
1. Créer les permissions (scopes) dans l'API
2. Créer les rôles
3. Assigner les permissions aux rôles
4. Assigner les rôles aux utilisateurs
5. Dans le code, vérifier les permissions du token

### 7.1 Créer les Scopes (Permissions)

**Aller dans l'API créée:**
1. Dashboard Auth0 > Applications > APIs
2. Cliquez sur votre API
3. Onglet "Scopes"

**Créer un scope:**
1. Cliquez sur "Create a new Scope" ou le bouton "+"
2. Entrez:
   - **Scope Name**: Identifiant unique (par exemple "read:documents")
   - **Description**: Description lisible (par exemple "Lire les documents")
3. Cliquez sur "Create"

**Scopes recommandés pour une application classique:**
```
read:documents    - Lire les documents
create:documents  - Créer des documents
update:documents  - Modifier les documents
delete:documents  - Supprimer les documents
read:users        - Lire les profils utilisateurs
admin:settings    - Accéder aux paramètres admin
```

### 7.2 Créer les Rôles

**Pour accéder aux rôles:**
1. Dashboard Auth0 > User Management > Roles
2. Cliquez sur "Create Role"

**Créer un rôle:**
1. Entrez:
   - **Name**: Nom du rôle (par exemple "Admin", "Editor", "Viewer")
   - **Description**: Description (par exemple "Administrateur avec tous les droits")
2. Cliquez sur "Create"

**Rôles recommandés:**

1. **Admin**
   - Tous les droits
   - Description: "Administrateur système"

2. **Editor**
   - Créer, lire, modifier les documents
   - Description: "Peut créer et modifier les documents"

3. **Viewer**
   - Seulement lire les documents
   - Description: "Peut consulter les documents"

### 7.3 Assigner les Permissions aux Rôles

**Pour chaque rôle:**
1. Dashboard Auth0 > User Management > Roles
2. Cliquez sur le rôle
3. Onglet "Permissions"
4. Cliquez sur "Add Permissions"
5. Sélectionnez votre API
6. Cochez les permissions (scopes) à ajouter au rôle
7. Cliquez sur "Add Permissions"

**Exemple pour Admin:**
- Sélectionnez ALL les scopes

**Exemple pour Editor:**
- read:documents
- create:documents
- update:documents

**Exemple pour Viewer:**
- read:documents

### 7.4 Assigner les Rôles aux Utilisateurs

**Manuellement via le dashboard:**
1. Dashboard Auth0 > User Management > Users
2. Cliquez sur l'utilisateur
3. Onglet "Roles"
4. Cliquez sur "Assign Roles"
5. Sélectionnez les rôles
6. Cliquez sur "Assign"

**Ou automatiquement via une action/règle:**
- À chaque login, assigner automatiquement un rôle
- Basé sur l'email ou d'autres critères

### 7.5 Inclure les rôles dans le token

**Pour que les rôles apparaissent dans le token JWT:**

**Option 1: Via les paramètres du tenant (simple)**
1. Dashboard Auth0 > Settings > Tenant Settings
2. Cherchez "RBAC Settings"
3. Activez "Enable RBAC" et "Add Permissions in the Access Token"

**Option 2: Via une action/règle (plus flexible)**
Cette option vous permet de personnaliser ce qui est inclus dans le token.

---

## 8. Flow de login/logout

### Vue conceptuelle du flux complet:

```
Utilisateur            Navigateur              Application Blazor      Auth0
   |                      |                         |                    |
   |---(1) Clique login----|                        |                    |
   |                       |(2) Navigue vers login  |                    |
   |                       |   endpoint ASP.NET     |                    |
   |                       |                        |                    |
   |                       |---(3) Redirige vers----|---> /authorize?   |
   |                       |       Auth0 login      |     client_id=...  |
   |                       |                        |                    |
   |                       |                        | (4) Affiche le     |
   |                       |                        |     formulaire     |
   |                       |                        |     de login       |
   |                       |     (5) Entre email/mdp                    |
   |                       |                        |---> Valide les    |
   |                       |                        |     credentials    |
   |                       |                        | (MFA si activé)    |
   |                       |                        |<--- Génère        |
   |                       |                        |     auth code      |
   |                       |<--- Redirige vers ------|---- +callback     |
   |                       |     localhost/signin...  |     URL avec code |
   |                       |                        |                    |
   |---(6) Navigateur reçoit authorization code ----|                    |
   |                       |                        |                    |
   |                       | (7) ASP.NET Core échange le code           |
   |                       |     contre un token (coulisse, backend)    |
   |                       |                        |---> POST /token   |
   |                       |                        |     code +        |
   |                       |                        |     client_secret |
   |                       |                        |<--- Access Token  |
   |                       |                        |     ID Token      |
   |                       |                        |     Refresh Token |
   |                       |                        |                    |
   |                       | (8) Créé la session utilisateur            |
   |                       |     (httponly cookie)                      |
   |                       |<--- Redirige vers ------|---- / (home)      |
   |                       |     homepage avec cookie                   |
   |                       |                        |                    |
   |---(9) Utilisateur connecté ---|                |                    |
   |                       |                        | Accès autorisé ✓  |
   |                       |                        |                    |
   |                       |<------- (10) Page de contenu ------         |
   |                       |                        |                    |

```

### Processus détaillé du LOGIN:

#### Phase 1: Initiation du login (Frontend)
1. Utilisateur clique sur le bouton "Login"
2. L'application Blazor appelle l'endpoint de login ASP.NET Core
3. L'endpoint redirige vers Auth0 avec les paramètres:
   - client_id
   - redirect_uri (votre callback URL)
   - response_type=code
   - scope (les permissions demandées)
   - state (token anti-CSRF)
   - nonce (pour la sécurité, si présent)

#### Phase 2: Authentification chez Auth0
1. Auth0 affiche le formulaire de login
2. Utilisateur entre son email et mot de passe
3. Auth0 valide les credentials
4. Si MFA est activé, demander la vérification (SMS, Email, Authenticator, etc.)
5. Auth0 génère un code d'autorisation (authorization code)
6. Auth0 redirige vers l'URL de callback avec le code

#### Phase 3: Échange du code (Backend)
1. ASP.NET Core reçoit le code d'autorisation
2. Le backend échange le code contre des tokens:
   - ACCESS TOKEN: Utilisé pour accéder aux APIs
   - ID TOKEN: Contient les informations de l'utilisateur
   - REFRESH TOKEN: Utilisé pour obtenir un nouvel access token
3. Le backend valide le token
4. Le backend crée une session (httponly cookie)

#### Phase 4: Redirection et conclusion
1. L'utilisateur est redirigé vers la page d'accueil
2. La session est maintenue via le cookie httponly
3. Les requêtes suivantes incluent le cookie
4. L'utilisateur est connecté

### Processus détaillé du LOGOUT:

#### Scénario 1: Logout simple (déconnexion de l'app uniquement)
```
Utilisateur            Navigateur              Application           Auth0
   |                      |                         |                    |
   |---(1) Clique logout---|                        |                    |
   |                       |                        |                    |
   |                       |(2) POST /logout ou     |                    |
   |                       |    GET /logout endpoint|                    |
   |                       |                        |                    |
   |                       |---(3) Détruit session--|-----|               |
   |                       |       (httponly cookie)|     Invalide       |
   |                       |                        |     session         |
   |                       |<-- (4) Redirige vers--|-----|               |
   |                       |       homepage                              |
   |                       |                        |                    |
   |---(5) Déconnecté ----|                        |                    |
```

#### Scénario 2: Logout avec Auth0 (Single Logout)
```
Utilisateur            Navigateur              Application           Auth0
   |                      |                        |                    |
   |---(1) Clique logout---|                       |                    |
   |                       |                       |                    |
   |                       |(2) Détruit session    |                    |
   |                       |                       |                    |
   |                       |---(3) Redirige vers---|-> /v2/logout      |
   |                       |       https://domain/ |   ?client_id=...   |
   |                       |       /v2/logout      |   &returnTo=...    |
   |                       |                       |                    |
   |                       |                       |<---(4) Invalide   |
   |                       |                       |       sessions     |
   |                       |<-- (5) Redirige vers--|---- returnTo URL  |
   |                       |       homepage                             |
   |                       |                        |                    |
   |---(6) Complètement déconnecté ---|           |                    |
```

**Différence importante:**
- Scénario 1: Déconnexion locale seulement (reste connecté à Auth0)
- Scénario 2: Déconnexion complète (quitte Auth0 aussi)

### Cas spécial: Refresh Token

Quand l'ACCESS TOKEN expire (généralement après 24-48h):

```
Application vient à expirer             Auth0
l'access token                          |
   |                                    |
   |---(1) Détecte expiration           |
   |       (401 Unauthorized)           |
   |                                    |
   |---(2) Envoie refresh token ----->--|
   |       POST /token                  |
   |       grant_type=refresh_token     |
   |       refresh_token=xyz            |
   |                                    |
   |<---(3) Génère nouveau ------       |
   |       access token                 |
   |       (même user, mêmes perms)    |
   |                                    |
   |---(4) Requête relancée -------->  |
        avec nouveau token
```

---

## 9. Stockage des credentials

### Pourquoi c'est important:

Les credentials Auth0 (Domain, Client ID, Client Secret) contiennent des informations sensibles qui pourraient permettre à quelqu'un d'accéder à votre système. Un stockage inadéquat pose des risques de sécurité critiques.

### Règles de base:

1. **Ne JAMAIS commiter les secrets dans Git**
   - Ajouter à .gitignore
   - Vérifier l'historique Git

2. **Ne JAMAIS exposer les secrets dans les logs ou les messages d'erreur**

3. **Traiter les secrets comme des mots de passe**

4. **Utiliser des configurations différentes par environnement**
   - Développement
   - Staging
   - Production

### 9.1 Développement local: appsettings.json

**Structure du fichier appsettings.json:**
```
{
  "Auth0": {
    "Domain": "votrenom.auth0.com",
    "ClientId": "votre_client_id_ici",
    "ClientSecret": "votre_client_secret_ici",
    "CallbackUrl": "http://localhost:7000/signin-auth0"
  }
}
```

**Points importants:**
- Fichier JSON structuré
- Placer dans la racine du projet
- Accessible par l'application au démarrage
- Chaque environnement peut avoir son fichier

**Fichiers par environnement:**
```
appsettings.json                  (valeurs par défaut)
appsettings.Development.json      (surcharge en développement)
appsettings.Staging.json          (surcharge en staging)
appsettings.Production.json       (surcharge en production)
```

**Configurer dans .gitignore:**
```
# Ignorer les fichiers de configuration avec secrets
appsettings.Production.json
appsettings.Staging.json
appsettings.*.local.json
*.local.json
```

**Charger la configuration en C#:**
- La configuration se charge automatiquement via le système d'options d'ASP.NET Core
- Utiliser IOptions<Auth0Options> ou IConfiguration
- Les valeurs peuvent être surchargées par des variables d'environnement

### 9.2 Production: Variables d'environnement

**Variables d'environnement via appsettings:**

Format de chaînage (colon becomes colon in env var):
```
Auth0:Domain           = variable d'environnement
Auth0:ClientId         = variable d'environnement
Auth0:ClientSecret     = variable d'environnement
Auth0:CallbackUrl      = variable d'environnement
```

**Nommer les variables d'environnement:**
```
AUTH0_DOMAIN=monapp.auth0.com
AUTH0_CLIENT_ID=a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6
AUTH0_CLIENT_SECRET=a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6_abc123XYZ
AUTH0_CALLBACK_URL=https://monapp.com/signin-auth0
```

**Sur Windows (via Command Prompt):**
```
setx AUTH0_DOMAIN "monapp.auth0.com"
setx AUTH0_CLIENT_ID "votre_id"
setx AUTH0_CLIENT_SECRET "votre_secret"
```

**Sur Linux/Mac (via Terminal):**
```
export AUTH0_DOMAIN="monapp.auth0.com"
export AUTH0_CLIENT_ID="votre_id"
export AUTH0_CLIENT_SECRET="votre_secret"
```

**Via fichier .env (attention!):**
- Créer un fichier .env à la racine
- Contient les variables d'environnement
- NE JAMAIS COMMITER DANS GIT
- Utiliser une librairie comme dotenv pour charger
- Mettre dans .gitignore

**Structure .env:**
```
AUTH0_DOMAIN=monapp.auth0.com
AUTH0_CLIENT_ID=votre_id
AUTH0_CLIENT_SECRET=votre_secret
AUTH0_CALLBACK_URL=https://monapp.com/signin-auth0
```

### 9.3 Infrastructure cloud: Secrets Manager

**Azure Key Vault (pour Azure):**
- Service de gestion des secrets
- Accès contrôlé via IAM
- Audit automatique de tous les accès
- Chiffrement au repos et en transit
- Rotation automatique possible

**AWS Secrets Manager (pour AWS):**
- Service de gestion des secrets
- Rotation automatique
- Intégration avec AWS Lambda et autres services
- Audit avec CloudTrail

**Configuration dans ASP.NET Core:**
- Utiliser le Key Vault dans appsettings.Production.json
- Ou via le système de configuration d'ASP.NET Core
- Les secrets sont chargés automatiquement à la démarrage

### 9.4 Comparaison des méthodes:

| Méthode | Sécurité | Facilité | Environnement | Notes |
|---------|----------|---------|---------------|-------|
| appsettings.json | Faible | Haute | Développement | NE PAS utiliser en prod |
| Variables d'env | Moyenne | Moyenne | Prod, Dev | Bonne pour serveurs simples |
| Key Vault | Très haute | Moyenne | Production | Recommandé pour prod |
| .env + dotenv | Faible | Haute | Développement | Jamais en production |

### 9.5 Checklist de sécurité:

- [ ] Client Secret jamais hardcodé
- [ ] Client Secret jamais commité dans Git
- [ ] appsettings.Production.json dans .gitignore
- [ ] Fichier .env dans .gitignore (s'il existe)
- [ ] Variables d'environnement utilisées en production
- [ ] Logs ne contiennent pas les secrets
- [ ] Accès aux secrets contrôlé (IAM, permissions)
- [ ] Audit des accès aux secrets en place
- [ ] Rotation régulière des secrets (recommandé)
- [ ] Encryption en transit (HTTPS) et au repos

---

## 10. Testing du login

### Préparation:

Avant de tester, assurez-vous que:
1. Votre application Blazor + ASP.NET Core compile et démarre
2. L'application tourne sur localhost:7000 (ou le port configured)
3. Les URLs de callback sont configurées dans Auth0 dashboard
4. Les credentials (Domain, Client ID, Secret) sont dans appsettings.json
5. Au moins un utilisateur test existe dans Auth0

### Test 1: Vérifier la configuration Auth0

**Objectif:** Vérifier que votre application est correctement déclarée dans Auth0

**Étapes:**
1. Aller au Auth0 Dashboard
2. Aller à Applications
3. Chercher votre application dans la liste
4. Cliquer dessus
5. Vérifier l'onglet Settings:
   - Domain est correct
   - Client ID est visible
   - Application Type: Regular Web Application
   - Credentials: Client Secret disponible
6. Vérifier les URLs:
   - Allowed Callback URLs contient http://localhost:7000/signin-auth0
   - Allowed Logout URLs contient http://localhost:7000
   - CORS allowed origins contient http://localhost:7000
7. Note de sécurité: Le Client Secret doit être confidentiel

### Test 2: Vérifier la configuration locale

**Objectif:** Vérifier que votre application a les bons credentials

**Étapes:**
1. Ouvrir appsettings.json
2. Vérifier la section Auth0:
   - Domain: Pas vide et format "votretenint.auth0.com"
   - ClientId: Pas vide et environ 32 caractères
   - ClientSecret: Pas vide et long
   - CallbackUrl: "http://localhost:7000/signin-auth0" ou compatible
3. Vérifier que les valeurs correspondent à Auth0 Dashboard
4. Ne pas modifier ces valeurs après cette vérification

### Test 3: Tester le bouton de login via URL

**Objectif:** Simuler manuellement le flux sans encore avoir de bouton

**Étapes:**
1. Démarrer l'application Blazor + ASP.NET Core
2. Dans le navigateur, aller à http://localhost:7000
3. Vous devez voir la page d'accueil
4. Naviguer manuellement vers http://localhost:7000/signin-auth0
   - Note: Normalement, ce ne sont pas accédé directement
   - Cela devrait soit:
     a) Vous rediriger vers Auth0 login
     b) Donner une erreur (pas de code fourni)
   - Les deux cas suggèrent que la configuration est en place

### Test 4: Tester le login complet

**Objectif:** Effectuer un login complète

**Prérequis:**
- Avoir une page avec un bouton "Login"
- Le bouton doit pointer vers le endpoint de login configuré

**Étapes:**
1. Aller à http://localhost:7000 (ou votre page d'accueil)
2. Cliquer sur le bouton "Login"
3. Vous devez être redirigé vers la page de login Auth0 (https://votretenint.auth0.com/...)
4. Vérifier:
   - Vous voyez le formulaire Auth0
   - L'URL commence par votre domaine Auth0
   - Les éléments de UI Auth0 sont visibles (logo, formulaire)
5. Entrer des credentials de test:
   - Email de test
   - Mot de passe de test
6. Cliquer sur "Log In"
7. Si MFA est activé, complétez la vérification
8. Vous devez être redirigé vers http://localhost:7000/signin-auth0?code=...
   - Cette URL apparaît brièvement
   - Le backend échange le code
9. Vous devez être redirigé vers la page d'accueil ou un dashboard
10. Vous devez voir votre nom/email visible (confirmation de login)

### Test 5: Vérifier les tokens

**Objectif:** Vérifier que les tokens sont générés correctement

**Étapes:**
1. Après login, ouvrir les DevTools (F12)
2. Aller à l'onglet "Network"
3. Chercher les requêtes POST vers votre backend
4. Chercher une requête vers /token ou /signin-auth0
5. Vérifier les headers:
   - La requête doit avoir Content-Type: application/x-www-form-urlencoded
6. Vérifier les réponses:
   - Le statut doit être 200 OK
   - La réponse contient access_token
7. Vérifier les cookies:
   - Onglet "Storage" ou "Cookies"
   - Il doit y avoir un cookie de session httponly
   - Name: AspNetCore.Identity.Application (ou similaire)
8. Vérifier les tokens via JWT.io:
   - Prendre le token JWT (si visible dans les logs dev)
   - Aller à https://jwt.io
   - Paster le token dans "Encoded"
   - Vérifier les claims:
     - sub (subject): ID de l'utilisateur
     - email: Email de l'utilisateur
     - iss (issuer): "https://votretenint.auth0.com/"
     - aud (audience): Votre Client ID

### Test 6: Vérifier les informations utilisateur

**Objectif:** Vérifier que les informations utilisateur sont correctement chargées

**Étapes:**
1. Après login réussi
2. Vérifier la page affiche:
   - Nom de l'utilisateur
   - Email de l'utilisateur
3. Vérifier dans le code:
   - User.Identity.IsAuthenticated retourne true
   - User.Identity.Name contient l'email
   - Les claims contiennent les informations attendues
4. Ouvrir le navigateur DevTools > Network
5. Vérifier les requêtes:
   - Les requêtes API doivent inclure l'Authorization header
   - Format: Authorization: Bearer [token]

### Test 7: Tester le logout

**Objectif:** Vérifier que la déconnexion fonctionne

**Étapes:**
1. Après login réussi (voir Test 4)
2. Cliquer sur le bouton "Logout" ou "Déconnexion"
3. Vérifier:
   - Vous êtes redirigé vers la page de logout
   - Les cookies de session sont supprimés
4. Revenir à la page d'accueil:
   - Vous devez être considéré comme non connecté
   - Le bouton "Login" doit réapparaître
5. Aller à la page /logout:
   - Vous devez être redirigé vers Auth0 logout
   - Vérifier l'URL: https://votretenint.auth0.com/v2/logout?...
6. Revenir à votre application:
   - Vous devez être complètement déconnecté
   - Pas de token en mémoire

### Test 8: Vérifier les rôles et permissions

**Objectif:** Vérifier que les rôles/permissions sont correctement inclus dans le token

**Étapes:**
1. Après login (voir Test 4)
2. Vérifier que l'utilisateur a des rôles assignés:
   - Auth0 Dashboard > Users
   - Cliquer sur votre utilisateur test
   - Onglet "Roles"
   - Vérifier que les rôles sont assignés
3. Vérifier que les rôles apparaissent dans le token:
   - Ouvrir DevTools
   - Chercher les logs ou la réponse de /signin-auth0
   - Copier le token JWT
   - Aller à jwt.io
   - Vérifier les claims:
     - "roles": ["Admin", "Editor"] (ou les rôles assignés)
4. Vérifier que les permissions/scopes sont dans le token:
   - Chercher le claim "permissions" ou "scope"
   - Vérifier qu'il contient les permissions associées aux rôles
5. Dans le code de l'API:
   - Vérifier que User.IsInRole("Admin") retourne true (si Admin assigné)
   - Vérifier que User.HasClaim("permissions", "read:documents") retourne true

### Test 9: Tester le Multi-Factor Authentication (MFA)

**Objectif:** Vérifier que MFA fonctionne correctement (si activé)

**Prérequis:**
- MFA activé dans Auth0
- Utilisateur de test avec MFA configurée

**Étapes:**
1. Aller au Auth0 Dashboard
2. Vérifier que MFA est activé:
   - Settings > Tenant Settings
   - Chercher "Multi-Factor Authentication"
   - Vérifier que au moins une option est coché (Email, SMS, Authenticator, etc.)
3. Configurer un utilisateur test pour MFA:
   - User Management > Users
   - Chercher l'utilisateur
   - Onglet "Identity Providers"
   - Ajouter une méthode de MFA si possible
4. Tester le login avec MFA:
   - Cliquer sur "Login"
   - Aller à Auth0 login
   - Entrer email et mot de passe
   - Vous devez voir une deuxième étape: "Verify with..."
   - Vérifier par SMS, Email, ou Authenticator selon la configuration
   - Entrer le code reçu
   - Vous devez être redirigé vers l'app

### Test 10: Tester les erreurs et edge cases

**Objectif:** Vérifier la gestion des erreurs

**Cas à tester:**

1. **Credentials invalides**
   - Aller au Auth0 login
   - Entrer email incorrect ou mot de passe incorrect
   - Vérifier: Message d'erreur clair

2. **Utilisateur désactivé**
   - Créer un utilisateur dans Auth0
   - Le désactiver (Disable User)
   - Tenter de le connecter
   - Vérifier: Message d'erreur approprié

3. **Callback URL incorrecte**
   - Modifier temporairement appsettings.json
   - Mettre CallbackUrl = "http://localhost:7001/signin-auth0" (mauvais port)
   - Tenter de login
   - Vérifier: Erreur Auth0 (invalid callback)
   - Restaurer la bonne URL

4. **Token expiré**
   - Si vous avez une API protégée:
   - Login
   - Attendre que le token expire (configurable)
   - Faire une requête à l'API protégée
   - Vérifier: Refresh token fonctionne ou erreur 401

5. **Client Secret incorrect**
   - Modifier temporairement le Client Secret dans appsettings.json
   - Tenter de login
   - Vérifier: Erreur lors de l'échange du token (backend)

### Test 11: Vérifier l'audit et les logs

**Objectif:** Vérifier que tout est enregistré correctement

**Étapes:**
1. Aller au Auth0 Dashboard
2. Cliquer sur "Logs" dans le menu de gauche
3. Vous devez voir un historique des événements:
   - s (Successful Login)
   - f (Failed Login)
   - ss (Success Signup)
   - fs (Failed Signup)
   - etc.
4. Cliquer sur chaque log pour voir les détails:
   - Timestamp
   - User ID
   - IP Address
   - User Agent (navigateur)
   - Statut de succès/échec
   - Détails de l'erreur (si échec)

### Test 12: Tester sur différents environnements

**Objectif:** S'assurer que ça fonctionne partout

**Environnements à tester:**

1. **Développement local**
   - http://localhost:7000
   - Navigateur: Chrome, Firefox, Safari
   - Vérifier: Fonctionnement complet

2. **Localhost avec port différent**
   - http://localhost:5000
   - Ajouter dans Auth0 Allowed URLs
   - Tester le login

3. **Machine distante (optionnel)**
   - Si vous avez un serveur de staging
   - https://staging.monapp.com
   - Ajouter l'URL dans Auth0
   - Tester le login complet

### Résolution de problèmes courants:

**Problème: Erreur "Invalid callback"**
- Solution: Vérifier que la callback URL dans appsettings.json correspond exactement à celle dans Auth0
- Vérifier aussi le protocole (http vs https) et le port

**Problème: Client Secret rejeté**
- Solution: Copier à nouveau le Client Secret depuis Auth0
- S'assurer qu'aucun espace avant/après

**Problème: Token contient une erreur "invalid_audience"**
- Solution: Vérifier que le Client ID est correct
- Vérifier que l'API identifier est correct

**Problème: Les rôles n'apparaissent pas dans le token**
- Solution: Vérifier que RBAC est activé dans le tenant
- Vérifier que l'utilisateur a des rôles assignés
- Vérifier que "Add Permissions in the Access Token" est activé

**Problème: Redirect loop ou refresh infini**
- Solution: Vérifier la configuration des endpoints de logout
- Vérifier que les tokens ne sont pas corrompus
- Vérifier la configuration de la session

---

## Checklist finale:

Avant de passer en production:

- [ ] Auth0 tenant créé avec le bon plan
- [ ] Application créée (Regular Web Application)
- [ ] Toutes les URLs configurées (Callback, Logout, CORS)
- [ ] Credentials sécurisés (pas hardcoded, dans env vars ou Key Vault)
- [ ] API créée avec les scopes appropriés
- [ ] Rôles et permissions configurés
- [ ] MFA activé (recommandé)
- [ ] Utilisateurs de test créés
- [ ] Tous les tests passent (1-12)
- [ ] Logs d'audit vérifiés
- [ ] Documentation mise à jour pour l'équipe
- [ ] Plan de rotation des secrets en place

---

## Ressources supplémentaires:

- **Auth0 Documentation:** https://auth0.com/docs
- **ASP.NET Core OpenID Connect:** https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/oauth2-personal-app
- **Blazor Authentication:** https://docs.microsoft.com/en-us/aspnet/core/blazor/security/
- **JWT (JSON Web Tokens):** https://jwt.io
- **OAuth 2.0 Specification:** https://tools.ietf.org/html/rfc6749
- **OpenID Connect Specification:** https://openid.net/specs/openid-connect-core-1_0.html

---

**Document créé:** 2026-02-18
**Version:** 1.0
**Auteur:** Guide Auth0 Setup

