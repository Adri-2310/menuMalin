# MenuMalin 🍽️

Une application web intelligente pour la gestion de recettes avec recherche API, favoris personnalisés et création de recettes utilisateur. Démonstration complète des principes de **Programmation Orientée Objet (POO)** et architecture moderne en ASP.NET Core + Blazor WebAssembly.

## 🚀 Quick Start

```bash
# Cloner et restaurer
git clone <repo-url> && cd menuMalin && dotnet restore

# Configurer & lancer
cd menuMalin.Server
dotnet ef database update
dotnet run
```

**Accès**:
- Frontend: https://localhost:7057
- API: https://localhost:7057/api

---

## 📚 Documentation

Toute la documentation détaillée se trouve dans le dossier `/docs`:

| Document | Contenu |
|----------|---------|
| 📖 [INSTALLATION.md](docs/INSTALLATION.md) | Configuration complète, base de données, secrets |
| 🏗️ [ARCHITECTURE.md](docs/ARCHITECTURE.md) | Structure, patterns POO, principes SOLID |
| ✨ [FEATURES.md](docs/FEATURES.md) | Fonctionnalités détaillées, cas d'usage |
| 🔌 [API.md](docs/API.md) | Endpoints, exemples, authentification |
| 🐛 [TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md) | Problèmes courants et solutions |
| 📝 [CHANGELOG.md](docs/CHANGELOG.md) | Historique des modifications |

---

## 👤 Auteur

**Adrien Mertens**
Examen - Programmation Orientée Objet - 2026

---

## 📊 Statistiques du Projet

- **Backend**: ASP.NET Core 9.0 + Entity Framework Core
- **Frontend**: Blazor WebAssembly + Bootstrap 5
- **Database**: MySQL 8.0
- **Build**: ✅ 0 erreurs, 0 avertissements
- **Code Quality**: Production-ready

---

## 🎯 Principes POO

✅ **Encapsulation** - Services avec interfaces
✅ **Abstraction** - DTOs et contracts
✅ **Héritage** - Hiérarchie d'exceptions
✅ **Polymorphisme** - Implémentations multiples
✅ **Composition** - Services injectés

👉 *Voir [ARCHITECTURE.md](docs/ARCHITECTURE.md) pour détails complets*

---

## ✨ Fonctionnalités

- 🔍 Recherche recettes via API TheMealDB
- ❤️ Gestion des favoris avec persistance BD
- 📝 Création/modification recettes personnalisées
- 👥 Partage public/privé de recettes
- 🔐 Authentification sécurisée (BCrypt + Cookies HttpOnly)
- 📱 Interface responsive (mobile-friendly)
- 🛡️ Gestion d'erreurs robuste (middleware global)
- 🔄 Retry automatique avec backoff exponentiel

---

## 🚀 Déploiement

Production deployment: *Voir [docs/](docs/)*

---

## 📧 Contact

**Email**: adrien.mertens@example.com
**Projet**: MenuMalin - Gestion Intelligente de Recettes
