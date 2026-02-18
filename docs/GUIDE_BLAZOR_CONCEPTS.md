# Guide Complet Blazor WebAssembly pour Débutants

## Table des matières

1. [Introduction à Blazor WebAssembly](#introduction)
2. [Blazor Server vs WebAssembly](#comparaison)
3. [Composants Blazor](#composants)
4. [Pages vs Composants](#pages-vs-composants)
5. [Binding des Données](#binding)
6. [Événements](#evenements)
7. [Lifecycle du Composant](#lifecycle)
8. [Services et Dependency Injection](#dependency-injection)
9. [Routing et Navigation](#routing)
10. [State Management](#state-management)
11. [Communication Backend](#communication-backend)
12. [AuthorizeView et Contrôle d'Accès](#authorize-view)

---

## 1. Introduction à Blazor WebAssembly {#introduction}

### Qu'est-ce que Blazor WebAssembly?

Blazor WebAssembly est un framework web développé par Microsoft qui permet aux développeurs C# de créer des applications web interactives côté client sans utiliser JavaScript. Blazor WebAssembly exécute le code C# directement dans le navigateur grâce à WebAssembly (une technologie standard du web).

### Caractéristiques principales

**WebAssembly (WASM)**
- Format binaire qui s'exécute dans le navigateur
- Très rapide et efficace
- Supporte le code compilé (C#, Rust, Go, etc.)
- Disponible dans tous les navigateurs modernes

**Avantages de Blazor WebAssembly**

| Avantage | Explication |
|----------|------------|
| **Même langage partout** | Utilisez C# côté client ET serveur |
| **Exécution côté client** | Pas de latence réseau pour chaque interaction |
| **Offline capable** | L'app peut fonctionner sans connexion internet |
| **Réutilisation de code** | Partagez la logique métier entre client et serveur |
| **Écosystème .NET** | Accédez aux milliers de NuGet packages |

**Inconvénients**

| Inconvénient | Explication |
|-------------|------------|
| **Taille initiale** | Le runtime .NET doit être téléchargé (~2-5MB compressé) |
| **Démarrage lent** | Chargement initial plus long que JavaScript |
| **Limitations navigateur** | Accès limité aux APIs du navigateur |
| **Pas d'accès direct à la base de données** | Doit passer par une API |

---

## 2. Blazor Server vs Blazor WebAssembly {#comparaison}

### Architecture générale

```
┌─────────────────────────────────────────────────────────────────┐
│                         BLAZOR SERVER                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  NAVIGATEUR                               SERVEUR               │
│  ┌─────────────────┐                    ┌──────────────────┐   │
│  │  HTML / UI      │◄────WebSocket────►│  Runtime .NET    │   │
│  │  Peu de traitement    (SignalR)     │  Logique métier  │   │
│  └─────────────────┘                    │  Accès BD        │   │
│                                         └──────────────────┘   │
│  Chaque interaction passe par le réseau                         │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                    BLAZOR WEBASSEMBLY                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  NAVIGATEUR                               SERVEUR               │
│  ┌──────────────────────────┐           ┌──────────────────┐   │
│  │  Runtime .NET (WASM)     │           │  API REST/gRPC   │   │
│  │  Logique métier          │──HTTP────►│  Accès BD        │   │
│  │  Rendering                │◄─────────┤                  │   │
│  └──────────────────────────┘           └──────────────────┘   │
│  Exécution locale, requêtes HTTP au besoin                      │
└─────────────────────────────────────────────────────────────────┘
```

### Comparaison détaillée

| Aspect | Blazor Server | Blazor WebAssembly |
|--------|----------------|-------------------|
| **Où s'exécute le code** | Sur le serveur | Dans le navigateur |
| **Connexion** | WebSocket (SignalR) persistante | HTTP (stateless) |
| **Latence UI** | Dépend de la latence réseau | Immédiate (côté client) |
| **Taille téléchargée** | Petite (HTML + JS) | Grande (~5MB) |
| **Accès à la BD** | Direct depuis le composant | Via une API |
| **Scalabilité serveur** | Demande beaucoup de ressources | Serveur léger |
| **Offline** | Non possible | Possible |
| **Déploiement** | Nécessite un serveur | Peut être une SPA statique |

### Quand utiliser quoi?

**Utilisez Blazor Server si:**
- Vous avez besoin d'une latence ultra-basse
- Votre application est complexe et nécessite beaucoup de calculs côté serveur
- Vous avez besoin d'accès direct à la base de données
- Bande passante limitée (petite taille téléchargée)

**Utilisez Blazor WebAssembly si:**
- Vous avez besoin d'offline capability
- Vous voulez réduire la charge serveur
- Vous pouvez héberger sur une CDN statique
- Les utilisateurs ont une bonne connexion
- Vous avez beaucoup d'utilisateurs (scalabilité)

---

## 3. Composants Blazor {#composants}

### Qu'est-ce qu'un composant?

Un composant Blazor est une unité réutilisable d'interface utilisateur. C'est un conteneur auto-contenu qui combine:
- La structure HTML
- La logique C#
- L'état (données)
- Les styles CSS

Un composant est similaire à une fonction ou une classe qui retourne une portion de l'UI.

### Anatomie d'un composant

```
┌─────────────────────────────────────────────┐
│         COMPOSANT BLAZOR (Counter)          │
├─────────────────────────────────────────────┤
│                                              │
│  1. MARKUP HTML                             │
│  ┌─────────────────────────────────────┐   │
│  │ <button @onclick="IncrementCount">  │   │
│  │   Click me: @count                  │   │
│  │ </button>                           │   │
│  └─────────────────────────────────────┘   │
│                                              │
│  2. LOGIQUE C# (@code)                      │
│  ┌─────────────────────────────────────┐   │
│  │ @code {                             │   │
│  │   int count = 0;                    │   │
│  │   void IncrementCount() { count++; }│   │
│  │ }                                   │   │
│  └─────────────────────────────────────┘   │
│                                              │
│  3. STYLE CSS (optionnel)                   │
│  ┌─────────────────────────────────────┐   │
│  │ <style>                             │   │
│  │   button { color: blue; }           │   │
│  │ </style>                            │   │
│  └─────────────────────────────────────┘   │
│                                              │
└─────────────────────────────────────────────┘
```

### Propriétés d'un composant

**État (State)**
- Variables qui stockent les données du composant
- Quand l'état change, le composant se re-rend automatiquement

**Paramètres (Parameters)**
- Données passées du composant parent au composant enfant
- Permettent la communication parent-enfant

**Événements (Events)**
- Des callbacks passés du parent à l'enfant
- Permettent l'enfant de notifier le parent

**Méthodes**
- Code C# pour traiter la logique

### Hiérarchie de composants

```
┌──────────────────────────────────────────────────┐
│              App.razor (Racine)                   │
├──────────────────────────────────────────────────┤
│                                                   │
│  ┌────────────────────┐   ┌──────────────────┐  │
│  │   MainLayout       │   │   Navigation     │  │
│  ├────────────────────┤   ├──────────────────┤  │
│  │                    │   │                  │  │
│  │ ┌──────────┐       │   │ ┌──────────────┐ │  │
│  │ │  Counter │       │   │ │   NavLinks   │ │  │
│  │ └──────────┘       │   │ └──────────────┘ │  │
│  │                    │   │                  │  │
│  │ ┌──────────┐       │   │                  │  │
│  │ │  Weather │       │   │                  │  │
│  │ └──────────┘       │   │                  │  │
│  └────────────────────┘   └──────────────────┘  │
│                                                   │
└──────────────────────────────────────────────────┘
```

---

## 4. Pages vs Composants {#pages-vs-composants}

### Pages (Razor Pages)

**Définition**
Les pages sont des composants spéciaux qui représentent une route complète dans l'application. Elles ont une URL directe et peuvent être navigables.

**Caractéristiques**
- Ont une directive `@page` qui définit la route
- Représentent une "vue complète" de l'application
- Peuvent recevoir des paramètres d'URL
- Sont navigables via les liens hypertext

**Exemple de route**
```
@page "/counter"
→ Accessible via: https://monsite.com/counter

@page "/product/{id:int}"
→ Accessible via: https://monsite.com/product/123
```

### Composants réutilisables

**Définition**
Les composants réutilisables sont des blocs d'UI sans route. Ils ne sont accessibles que via inclusion dans d'autres composants.

**Caractéristiques**
- N'ont PAS de directive `@page`
- Sont réutilisables dans plusieurs pages
- Reçoivent des données via les paramètres
- Notifient le parent via des événements

### Différences visuelles

```
┌─────────────────────────────────────────────────┐
│                      PAGE                        │
├─────────────────────────────────────────────────┤
│ @page "/counter"                                │
│ @page "/counter/{id:int}"  ← Routes            │
│                                                  │
│ <h1>Compteur</h1>                               │
│ <Counter @ref="counter" />  ← Inclut des       │
│ <Weather />                     composants      │
│                                                  │
└─────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────┐
│                   COMPOSANT                      │
├─────────────────────────────────────────────────┤
│ (Pas de @page)                                   │
│                                                  │
│ @if (ShowContent)                               │
│ {                                                │
│   <div class="card">                            │
│     @ChildContent                               │
│   </div>                                         │
│ }                                                │
│                                                  │
│ @code {                                          │
│   [Parameter] public bool ShowContent { get; set; } │
│ }                                                │
│                                                  │
└─────────────────────────────────────────────────┘
```

### Structure recommandée du projet

```
MonApp/
├── Pages/                    ← Pages avec @page
│   ├── Index.razor
│   ├── Counter.razor
│   └── Product.razor
│
├── Components/              ← Composants réutilisables
│   ├── Header.razor
│   ├── Footer.razor
│   ├── ProductCard.razor
│   └── Shared/
│       ├── MainLayout.razor
│       └── NavMenu.razor
│
├── Services/                ← Logique métier
│   └── ApiService.cs
│
├── Models/                  ← Classes de données
│   └── Product.cs
│
└── App.razor               ← Composant racine
```

---

## 5. Binding des Données {#binding}

### Qu'est-ce que le binding?

Le binding (ou liaison de données) est un mécanisme qui crée une connexion automatique entre les données (variables C#) et l'interface utilisateur (HTML). Quand les données changent, l'UI se met à jour automatiquement, et vice-versa.

### Types de binding

**One-way binding (uni-directionnel)**
- Les données du C# → HTML
- L'UI affiche les données
- Les changements d'UI ne reviennent pas à C#

```
Exemple: @count
┌──────────────────────────────────────┐
│  Variable C# (count = 5)             │
│          ↓                           │
│  Affichage HTML (texte "5")          │
└──────────────────────────────────────┘
```

**Two-way binding (bi-directionnel)**
- Les données du C# ↔ HTML
- L'UI affiche les données ET reflète les changements utilisateur
- Les changements UI mettent à jour les variables C#

```
Exemple: @bind="name"
┌────────────────────────────────────────┐
│  Variable C# (name = "Jean")           │
│          ↕  ↕                          │
│  Input HTML (affiche "Jean")           │
│                                         │
│  1. Utilisateur tape dans le champ     │
│     → Variable C# se met à jour        │
│  2. Code change la variable C#         │
│     → Input se met à jour              │
└────────────────────────────────────────┘
```

### Directive @bind

**Syntaxe basique**
```
@bind="nomVariable"
```

Cela crée automatiquement:
- Un affichage initial de la valeur
- Un événement change listener
- Une mise à jour de la variable quand l'utilisateur change la valeur

**Binding sur différents types d'éléments**

| Élément | Utilisation | Détail |
|---------|------------|--------|
| Input text | `@bind="name"` | Récupère le texte saisi |
| Input number | `@bind="age"` | Récupère le nombre saisi |
| Select | `@bind="selectedOption"` | Récupère la sélection |
| Textarea | `@bind="message"` | Récupère du texte multiligne |
| Checkbox | `@bind="isActive"` | Récupère true/false |
| Radio buttons | `@bind="choix"` | Récupère l'option sélectionnée |

### Options avancées du binding

**Binding avec event personnalisé**
Au lieu d'utiliser l'événement par défaut, vous pouvez spécifier un événement custom.

```
@bind:event="onkeyup"
```

Cela met à jour la variable à chaque keyup au lieu d'attendre la perte de focus.

**Binding avec conversion de type**
Quand les types ne correspondent pas exactement, Blazor essaie de convertir.

```
Variable C#: int age
HTML: <input type="text" @bind="age" />

"25" (string du HTML) → 25 (int en C#)
```

### Flow de données du binding

```
SCENARIO: Utilisateur tape "Alice" dans un <input @bind="name">

Étape 1: RENDU INITIAL
┌────────────────┐
│ name = "Bob"   │ (Variable C#)
│      ↓         │
│ value="Bob"    │ (Input HTML)
└────────────────┘

Étape 2: UTILISATEUR TAPE
┌────────────────┐
│ (Utilisateur tape "Alice")
│      ↓
│ value="Alice"  │ (HTML mis à jour localement)
└────────────────┘

Étape 3: ÉVÉNEMENT CHANGE
┌────────────────┐
│ L'événement "change" se déclenche
│      ↓
│ Envoi à C#: "Alice"
└────────────────┘

Étape 4: C# MIS À JOUR
┌────────────────┐
│ name = "Alice" │ (Variable C# changée)
│      ↓         │
│ StateHasChanged() (Composant re-rendu)
└────────────────┘

Étape 5: NOUVEL AFFICHAGE
┌────────────────┐
│ value="Alice"  │ (Input affiche la nouvelle valeur)
└────────────────┘
```

### Performance du binding

**Attention**
Le binding bi-directionnel crée une communication entre le navigateur et Blazor à chaque changement. Pour les applications WebAssembly, cela peut avoir un impact.

**Optimisations**
- Utilisez `@bind:event="onchange"` pour n'avoir des mises à jour qu'à la perte de focus
- Utilisez le one-way binding `@` quand vous n'avez besoin que d'afficher
- Pour les grandes listes, considérez un binding manuel avec événements explicites

---

## 6. Événements {#evenements}

### Qu'est-ce qu'un événement?

Un événement est une action utilisateur ou du système qui déclenche une réaction dans l'application. Exemples: cliquer sur un bouton, changer le contenu d'un input, soumettre un formulaire, charger une page.

### Système d'événements Blazor

Blazor capture les événements DOM (événements du navigateur) et les relie à des méthodes C#. Quand un événement se déclenche dans le HTML, une méthode C# est appelée.

```
┌──────────────────────────────────────────────┐
│         FLUX D'UN ÉVÉNEMENT                   │
├──────────────────────────────────────────────┤
│                                               │
│  1. L'utilisateur clique sur le bouton       │
│     ┌──────────────────┐                     │
│     │ <button>Cliquez</button>                │
│     └──────────────────┘                     │
│            ↓                                  │
│                                               │
│  2. Événement "click" détecté par le DOM    │
│            ↓                                  │
│                                               │
│  3. Blazor intercepte l'événement            │
│            ↓                                  │
│                                               │
│  4. Méthode C# appelée                       │
│     HandleClick() { ... }                    │
│            ↓                                  │
│                                               │
│  5. Composant re-rendu si state changé      │
│            ↓                                  │
│                                               │
│  6. Navigateur met à jour le DOM            │
│                                               │
└──────────────────────────────────────────────┘
```

### Les événements les plus courants

| Événement | Déclenchement | Utilisation typique |
|-----------|---------------|-------------------|
| `@onclick` | Clic sur un élément | Boutons, cliquables |
| `@onchange` | Changement d'un input | Inputs, selects, radios |
| `@onblur` | Perte de focus | Validation de formulaire |
| `@onfocus` | Gain de focus | Mise en évidence |
| `@onkeydown` | Touche enfoncée | Raccourcis clavier |
| `@onkeyup` | Touche relâchée | Recherche en temps réel |
| `@onsubmit` | Soumission formulaire | Envoi de données |
| `@onmouseover` | Souris sur l'élément | Hover effects |
| `@onmouseout` | Souris quitte l'élément | Fermeture de menu |
| `@onload` | Élément chargé | Initialisation |

### Syntaxe des événements

**Événement simple**
```
@onclick="NomMethode"
```
Appelle une méthode sans paramètres.

**Événement avec lambda**
```
@onclick="() => NomMethode()"
```
Permet d'appeler une méthode avec ou sans paramètres.

**Événement avec paramètres d'événement**
```
@onclick="HandleClick"
```
Et en C#:
```
void HandleClick(MouseEventArgs e)
{
    // e.ClientX, e.ClientY = position souris
}
```

**Événement async**
```
@onclick="HandleClickAsync"
```
La méthode peut être asynchrone (retourne Task).

### Événements sur les inputs

**onchange vs oninput**

| Événement | Déclenchement | Utilisation |
|-----------|---------------|------------|
| `@onchange` | À la perte de focus | Validation complète |
| `@oninput` | À chaque frappe | Recherche en temps réel |

```
Utilisateur tape dans un <input>

@onchange:
┌────────────────────────────────────────┐
│ Tape "H" → Rien ne se passe            │
│ Tape "i" → Rien ne se passe            │
│ Tape "!" → Rien ne se passe            │
│ Clique ailleurs → onchange déclenché   │
└────────────────────────────────────────┘

@oninput:
┌────────────────────────────────────────┐
│ Tape "H" → oninput déclenché           │
│ Tape "i" → oninput déclenché           │
│ Tape "!" → oninput déclenché           │
└────────────────────────────────────────┘
```

### Prévention du comportement par défaut

Blazor permet d'empêcher le comportement par défaut d'un événement.

**Exemple: Formulaire non soumis automatiquement**

Normalement, soumettre un formulaire recharge la page. Avec Blazor, vous pouvez l'empêcher pour traiter les données en C#.

```
@onsubmit="HandleSubmit"
@onsubmit:preventDefault="true"
```

---

## 7. Lifecycle du Composant {#lifecycle}

### Qu'est-ce que le lifecycle?

Le lifecycle (ou cycle de vie) d'un composant est la série d'étapes par lesquelles passe un composant depuis sa création jusqu'à sa destruction. À chaque étape, des événements se déclenchent, ce qui permet d'exécuter du code au moment approprié.

### Diagramme du cycle de vie

```
┌─────────────────────────────────────────────┐
│        CRÉATION DU COMPOSANT                │
│ (Composant instancié pour la première fois)│
└─────────────────────────────────────────────┘
             ↓
┌─────────────────────────────────────────────┐
│   SetParametersAsync()                      │
│ (Les paramètres reçus du parent sont définis)│
└─────────────────────────────────────────────┘
             ↓
┌─────────────────────────────────────────────┐
│   OnInitialized() ou OnInitializedAsync()   │
│ (Code d'initialisation)                     │
│ (Se déclenche une fois au démarrage)        │
│ (Ici: charger les données initiales)        │
└─────────────────────────────────────────────┘
             ↓
┌─────────────────────────────────────────────┐
│   OnParametersSet() ou OnParametersSetAsync()│
│ (Les paramètres ont changé)                 │
│ (Se déclenche chaque fois que @Parameter    │
│  change)                                    │
│ (Ici: charger les données dépendantes)     │
└─────────────────────────────────────────────┘
             ↓
┌─────────────────────────────────────────────┐
│        RENDU DU COMPOSANT                   │
│ (BuildRenderTree - génère le HTML)          │
└─────────────────────────────────────────────┘
             ↓
┌─────────────────────────────────────────────┐
│   OnAfterRender() ou OnAfterRenderAsync()   │
│ (Après le rendu, interactions avec le DOM)  │
│ (Ici: initialiser des plugins JS)           │
└─────────────────────────────────────────────┘
             ↓
┌─────────────────────────────────────────────┐
│        COMPOSANT ACTIF                      │
│ (Attend les événements utilisateur)         │
│ (États et interactions)                     │
└─────────────────────────────────────────────┘
             ↓
   (Événement utilisateur se déclenche)
             ↓
┌─────────────────────────────────────────────┐
│        ÉTAT CHANGE                          │
│ (Une variable change, un événement se       │
│  déclenche)                                 │
└─────────────────────────────────────────────┘
             ↓
┌─────────────────────────────────────────────┐
│        RE-RENDU                             │
│ (BuildRenderTree est appelé de nouveau)     │
└─────────────────────────────────────────────┘
             ↓
┌─────────────────────────────────────────────┐
│   OnAfterRender(firstRender=false)          │
│ (Après chaque re-rendu)                     │
└─────────────────────────────────────────────┘
             ↓
   (Boucle: État change → Re-rendu → AfterRender)
             ↓
┌─────────────────────────────────────────────┐
│        DESTRUCTION DU COMPOSANT             │
│ (Composant retiré de l'arborescence)        │
│ (Nettoyage des ressources)                  │
└─────────────────────────────────────────────┘
```

### Méthodes du cycle de vie

#### OnInitialized / OnInitializedAsync

**Quand?** Une fois, lors de la première création du composant

**Utilité**
- Charger les données initiales
- Initialiser les valeurs
- Lancer une requête API une seule fois

**Important**
- Ne se déclenche qu'une fois dans la vie du composant
- Idéal pour: initialiser les données de la page
- N'a pas accès aux paramètres modifiés (utiliser OnParametersSet pour ça)

#### OnParametersSet / OnParametersSetAsync

**Quand?** Quand les paramètres du composant changent (y compris la première fois)

**Utilité**
- Réagir aux changements de paramètres reçus du parent
- Charger de nouvelles données basées sur les paramètres
- Réinitialiser le composant quand un paramètre clé change

**Exemple d'utilisation**
```
Parent passe un ID produit en paramètre
ID change → OnParametersSet déclenché
Charger les données du nouveau produit
Afficher le nouveau produit
```

#### OnAfterRender / OnAfterRenderAsync

**Quand?** Après chaque rendu (y compris le premier)

**Utilité**
- Manipuler le DOM après le rendu
- Initialiser des plugins JavaScript
- Faire des appels JavaScript
- Détecter les changements visuels

**Important**
- À ce point, le DOM est à jour
- Utilisé rarement (surtout pour interagir avec JS)
- Ne devrait pas modifier l'état (cause une boucle infinie)

#### Dispose

**Quand?** Quand le composant est détruit

**Utilité**
- Nettoyer les ressources (timers, subscriptions)
- Fermer les connexions
- Libérer la mémoire

**Important**
- Critique pour éviter les fuites mémoire
- Implémentez `IAsyncDisposable` si vous avez du code async

### Comparaison des méthodes

| Méthode | Fois appelée | Premier render | Après paramètre change | Async? |
|---------|------------|----------------|----------------------|--------|
| OnInitialized | 1 fois | ✓ | ✗ | Non |
| OnInitializedAsync | 1 fois | ✓ | ✗ | Oui |
| OnParametersSet | À chaque paramètre change | ✓ | ✓ | Non |
| OnParametersSetAsync | À chaque paramètre change | ✓ | ✓ | Oui |
| OnAfterRender | À chaque rendu | ✓ | ✓ | Non |
| OnAfterRenderAsync | À chaque rendu | ✓ | ✓ | Oui |
| Dispose | À la destruction | ✗ | ✗ | Non |

### Scénarios pratiques

**Scénario 1: Charger une liste de produits une seule fois**
```
→ Utiliser: OnInitializedAsync
Pourquoi: Les données ne changent pas pendant la vie du composant
```

**Scénario 2: Afficher les détails d'un produit basé sur son ID**
```
→ Utiliser: OnParametersSetAsync
Pourquoi: L'ID (paramètre) peut changer, besoin de recharger les détails
```

**Scénario 3: Initialiser un graphique JavaScript**
```
→ Utiliser: OnAfterRenderAsync (avec firstRender == true)
Pourquoi: Le DOM doit être prêt avant d'initialiser le JS
```

**Scénario 4: Nettoyer un timer ou un listener**
```
→ Utiliser: Dispose
Pourquoi: Éviter les fuites mémoire
```

---

## 8. Services et Dependency Injection {#dependency-injection}

### Qu'est-ce qu'un service?

Un service est une classe qui encapsule une logique métier réutilisable. Au lieu de mettre toute la logique dans les composants, on crée des services pour:
- Communiquer avec une API
- Accéder à la base de données
- Gérer l'authentification
- Manipuler des données
- Gérer l'état global

**Avantage:** Réutilisabilité et séparation des préoccupations

### Qu'est-ce que l'injection de dépendance (DI)?

L'injection de dépendance est un pattern qui fournit automatiquement les services à ceux qui en ont besoin, sans avoir à les instancier manuellement.

```
┌──────────────────────────────────────┐
│     SANS INJECTION DE DÉPENDANCE     │
├──────────────────────────────────────┤
│                                      │
│ Composant:                           │
│ → Crée un service: new ApiService()  │
│ → Utilise le service                 │
│ → Instanciation manuel               │
│ → Difficile à tester (dépendances)   │
│                                      │
└──────────────────────────────────────┘

┌──────────────────────────────────────┐
│     AVEC INJECTION DE DÉPENDANCE     │
├──────────────────────────────────────┤
│                                      │
│ Configuration (Program.cs):          │
│ → "Enregistrer" le service           │
│                                      │
│ Conteneur DI:                        │
│ → Gère les instances                 │
│ → Fournit les services automatiquement│
│                                      │
│ Composant:                           │
│ → Reçoit le service injecté          │
│ → Utilise le service                 │
│ → Pas d'instanciation manuel         │
│ → Facile à tester (mock du service)  │
│                                      │
└──────────────────────────────────────┘
```

### Enregistrement des services

Les services sont enregistrés au démarrage de l'application, dans `Program.cs`.

**Exemple d'enregistrement**
```
Les services sont ajoutés avant le build
Forme générale: builder.Services.Add*(ServiceType, Implementation)
```

### Cycle de vie des services

Les services peuvent avoir différents cycles de vie:

| Cycle de vie | Description | Utilisation |
|--------------|-------------|------------|
| **Transient** | Nouvelle instance à chaque fois | Services stateless, légers |
| **Scoped** | Une instance par requête/page | Authentification, état page |
| **Singleton** | Une seule instance pour toute l'app | Cache global, config |

```
┌────────────────────────────────────────────┐
│         CYCLE DE VIE TRANSIENT             │
├────────────────────────────────────────────┤
│                                            │
│  Composant A demande ApiService            │
│  → Crée nouvelle instance 1               │
│                                            │
│  Composant B demande ApiService            │
│  → Crée nouvelle instance 2               │
│                                            │
│  Même composant demande ApiService         │
│  → Crée nouvelle instance 3               │
│                                            │
│  Plusieurs instances différentes           │
│                                            │
└────────────────────────────────────────────┘

┌────────────────────────────────────────────┐
│         CYCLE DE VIE SCOPED                │
├────────────────────────────────────────────┤
│                                            │
│  Requête HTTP 1:                          │
│  → Crée instance 1 pour toute la requête  │
│  → Composant A reçoit instance 1          │
│  → Composant B reçoit instance 1 (même)   │
│                                            │
│  Requête HTTP 2:                          │
│  → Crée instance 2 pour cette requête     │
│  → Composant C reçoit instance 2          │
│                                            │
│  Une instance par requête/page             │
│                                            │
└────────────────────────────────────────────┘

┌────────────────────────────────────────────┐
│         CYCLE DE VIE SINGLETON             │
├────────────────────────────────────────────┤
│                                            │
│  Démarrage: Crée instance 1                │
│                                            │
│  Composant A demande → instance 1          │
│  Composant B demande → instance 1 (même)   │
│  Composant C demande → instance 1 (même)   │
│                                            │
│  Une seule instance pour toute l'app       │
│                                            │
└────────────────────────────────────────────┘
```

### Utilisation d'un service dans un composant

Pour utiliser un service dans un composant:

1. **Injecter** le service via l'attribut `@inject`
2. **Utiliser** le service dans les méthodes du composant

```
Structure:
@inject ServiceType NomVariable

Blazor reconnaît @inject et fournit le service automatiquement
```

### Architecture typique avec services

```
┌─────────────────────────────────────────────────┐
│               COMPOSANT BLAZOR                   │
│ (Pages/Components)                              │
├─────────────────────────────────────────────────┤
│                                                  │
│  @inject ApiService api                         │
│  @inject AuthService auth                       │
│                                                  │
│  Utilise: await api.GetProducts()               │
│           auth.CurrentUser                      │
│                                                  │
└─────────────────────────────────────────────────┘
             ↓ Utilise
┌─────────────────────────────────────────────────┐
│              SERVICES (Services/)                │
├─────────────────────────────────────────────────┤
│                                                  │
│  ApiService:                                    │
│  → GET /api/products                            │
│  → POST /api/products                           │
│                                                  │
│  AuthService:                                   │
│  → Gérer l'authentification                     │
│  → Stockage du token                            │
│                                                  │
│  ProductService:                                │
│  → Logique métier produits                      │
│                                                  │
└─────────────────────────────────────────────────┘
             ↓ Utilise
┌─────────────────────────────────────────────────┐
│          SOURCES EXTERNES                        │
├─────────────────────────────────────────────────┤
│                                                  │
│  API REST                                       │
│  Base de données                                │
│  Services externes                              │
│                                                  │
└─────────────────────────────────────────────────┘
```

### Avantages de la DI

| Avantage | Explication |
|----------|------------|
| **Réutilisabilité** | Même service utilisé dans plusieurs composants |
| **Testabilité** | Facile de remplacer par un mock |
| **Flexibilité** | Changer d'implémentation sans modifier les composants |
| **Maintenabilité** | Logique centralisée et organisée |
| **Découplage** | Composants ne connaissent pas les détails d'implémentation |

---

## 9. Routing et Navigation {#routing}

### Qu'est-ce que le routing?

Le routing (ou routage) est le mécanisme qui mappe les URLs aux composants. Quand un utilisateur navigue vers une URL, le routeur détermine quel composant afficher.

### Système de routing Blazor

```
┌────────────────────────────────────────────┐
│          UTILISATEUR ACCÈDE À UNE URL      │
│  https://monsite.com/products/123          │
└────────────────────────────────────────────┘
             ↓
┌────────────────────────────────────────────┐
│  ROUTEUR BLAZOR INTERCEPTE                 │
│  (Composant Router dans App.razor)          │
└────────────────────────────────────────────┘
             ↓
┌────────────────────────────────────────────┐
│  ANALYSE L'URL                             │
│  /products/123 → Cherche @page "/products/{id:int}"
└────────────────────────────────────────────┘
             ↓
┌────────────────────────────────────────────┐
│  CORRESPONDANCE TROUVÉE                    │
│  Route: /products/{id:int}                 │
│  Composant: ProductDetail.razor            │
│  Paramètre: id = 123                       │
└────────────────────────────────────────────┘
             ↓
┌────────────────────────────────────────────┐
│  CHARGE LE COMPOSANT                       │
│  ProductDetail.razor chargé                │
│  Paramètre "id" reçu: 123                  │
└────────────────────────────────────────────┘
             ↓
┌────────────────────────────────────────────┐
│  RENDU                                     │
│  Affiche le produit avec ID 123            │
└────────────────────────────────────────────┘
```

### Définir une route

Les routes sont définies avec la directive `@page`.

**Routes simples**
```
@page "/counter"
Accessible via: /counter

@page "/about"
Accessible via: /about
```

**Routes avec paramètres**
```
@page "/product/{id:int}"
Accessible via: /product/123, /product/456
Paramètre "id" reçu comme entier

@page "/user/{name}"
Accessible via: /user/john, /user/alice
Paramètre "name" reçu comme string
```

**Routes multiples pour un même composant**
```
@page "/home"
@page "/"
Le même composant accessible de deux URL différentes
```

### Types de paramètres

Les paramètres d'URL peuvent avoir des types spécifiques:

| Type | Syntaxe | Exemple | Reçu en C# |
|------|---------|---------|-----------|
| String (défaut) | `{name}` | `/user/alice` | "alice" |
| Integer | `{id:int}` | `/product/123` | 123 |
| Boolean | `{active:bool}` | `/items/true` | true |
| Guid | `{id:guid}` | `/order/550e8400...` | Guid |
| Datetime | `{date:datetime}` | `/events/2024-01-15` | DateTime |

### Contraintes de route

Les contraintes limitent quelles URLs correspondent à une route:

**Exemple**
```
@page "/product/{id:int}"
→ Accepte: /product/123 ✓
→ Rejette: /product/abc ✗
→ Rejette: /product/12.5 ✗
```

### Navigation par code

Au lieu de cliquer sur un lien, on peut naviguer programmatiquement en C#.

**Utilisation typique**
- Redirection après une action
- Navigation conditionnelle
- Retour à la page précédente

**Accès au NavigationManager**
```
@inject NavigationManager nav
```

**Méthodes de navigation**
- `nav.NavigateTo("/page")` → Navigation simple
- `nav.NavigateTo("/page", forceLoad: true)` → Recharge la page
- `nav.NavigateTo("/login", replace: true)` → Remplace dans l'historique

### Composant NavLink

Le composant `NavLink` est un élément spécial qui:
- Crée automatiquement un lien
- Ajoute la classe CSS "active" quand la route actuelle correspond
- Améliore l'expérience utilisateur en montrant la page active

```
NavLink vs lien HTML normal:

<a href="/products">Produits</a>
→ Lien simple, pas de statut actif

<NavLink href="/products">Produits</NavLink>
→ Classe "active" si actuellement sur /products
→ Montre visuellement la page courante
```

### Historique et navigation en arrière

**Avant/Après en navigation**

```
Utilisateur sur /products
      ↓
Clique sur "Détails" → /products/123
      ↓
Clique sur "Retour" → /products (retour dans l'historique)
      ↓
Clique sur "Accueil" → / (nouvelle navigation)
```

Le navigateur gère automatiquement l'historique, comme une application web normale.

### Interception de navigation

On peut exécuter du code avant de quitter une page (par exemple: sauvegarder les changements).

**Utilité**
- Demander confirmation avant de quitter une page avec des changements
- Sauvegarder les données brouillons
- Alerter l'utilisateur

---

## 10. State Management {#state-management}

### Qu'est-ce que le state management?

Le state management est la gestion de l'état (données) de l'application. À mesure qu'une application grandit, gérer l'état devient complexe:
- Plusieurs composants doivent accéder aux mêmes données
- Les changements dans un composant doivent se refléter dans d'autres
- Il faut éviter les incohérences de données

```
┌────────────────────────────────────────┐
│     PETIT PROJET (State simple)        │
├────────────────────────────────────────┤
│                                        │
│  Chaque composant gère son état local  │
│  ✓ Pas de complexité                  │
│  ✓ Composants indépendants            │
│  ✗ Données dupliquées                 │
│                                        │
└────────────────────────────────────────┘

┌────────────────────────────────────────┐
│   GRAND PROJET (State complexe)       │
├────────────────────────────────────────┤
│                                        │
│  Besoin d'état partagé                │
│  ✓ Source unique de vérité            │
│  ✓ Synchronisation automatique        │
│  ✗ Complexité ajoutée                 │
│                                        │
└────────────────────────────────────────┘
```

### Niveaux du state

**1. State Local (composant seul)**
```
Variables locales du composant
Exemple: int count dans Counter.razor
```

**2. State Partagé (parent → enfant)**
```
Paramètres passés du parent à l'enfant
Événements remontant de l'enfant au parent
```

**3. State Global (application entière)**
```
Données accessibles partout
Exemple: utilisateur connecté, thème, panier d'achat
```

### Propagation du state

```
NIVEAU 1: STATE LOCAL
┌─────────────────────────┐
│   Counter.razor         │
│   int count = 0         │
│   (Local seulement)     │
└─────────────────────────┘

NIVEAU 2: STATE PARTAGÉ (Parent-Enfant)
┌─────────────────────────────────────────┐
│   Parent.razor                          │
│   int sharedCount = 0                   │
│   ┌──────────────────────────────────┐  │
│   │ <Child @bind="sharedCount" />    │  │
│   └──────────────────────────────────┘  │
│   Change: notifie enfant               │
│   ┌──────────────────────────────────┐  │
│   │ Child.razor                       │  │
│   │ @bind reçoit la valeur            │  │
│   │ Enfant notifie parent via événement│ │
│   └──────────────────────────────────┘  │
└─────────────────────────────────────────┘

NIVEAU 3: STATE GLOBAL
┌──────────────────────────────────────────┐
│   AppState Service (Singleton)           │
│   ┌────────────────────────────────────┐ │
│   │ CurrentUser                        │ │
│   │ ShoppingCart                       │ │
│   │ Theme                              │ │
│   └────────────────────────────────────┘ │
│                                          │
│   Injecté partout, accessible de n'importe où│
│   ┌────────────┐ ┌────────────┐ ┌────────────┐│
│   │ Header     │ │ Sidebar    │ │ Footer     ││
│   │ @inject... │ │ @inject... │ │ @inject... ││
│   └────────────┘ └────────────┘ └────────────┘│
└──────────────────────────────────────────┘
```

### Patterns de gestion d'état

**1. Parent comme source de vérité**

Le composant parent gère l'état et le passe aux enfants via paramètres.

```
Flux:
Parent.State → Child1.Parameter
           → Child2.Parameter

Changement:
Child1 → Événement → Parent → State change → Redraw all

Avantages: Simple, facile à comprendre
Inconvénients: Composants trop accouplés, prop drilling
```

**2. Service singleton partagé**

Un service global gère l'état, injecté partout.

```
Flux:
Service.State ← Injectable dans n'importe quel composant

Changement:
Composant A → Appelle Service.Update() → Service.State change
Composant B → Reçoit notification (si subscribé)

Avantages: Flexible, découplé
Inconvénients: Plus complexe, besoin de notifications
```

**3. Callback pattern**

Les enfants notifient le parent via des événements quand le state change.

```
Flux:
Parent → Passe State + EventCallback → Child
Child → État change → Appelle EventCallback
Parent → Callback reçu → Redraw

Avantages: Communication claire
Inconvénients: Verbose, beaucoup de boilerplate
```

### Problèmes courants du state management

**Le "Prop Drilling"**
```
Besoin d'une variable au plus profond niveau
Root
  → Level1 (pass prop)
    → Level2 (pass prop)
      → Level3 (pass prop)
        → Level4 (ENFIN utilise la prop)

Chaque niveau doit passer la prop même s'il ne l'utilise pas
Beaucoup de boilerplate, difficile à maintenir
```

**Synchronisation de données**
```
Deux composants affichent les mêmes données
Composant A modifie les données
Composant B ne voit pas la modification
Données incohérentes
```

**Performance**
```
State change → Composant re-rendu
Mais aussi tous les composants enfants re-rendus
Même ceux qui n'utilisent pas le state changé
Perte de performance potentielle
```

### Solutions recommandées par niveau

| Complexité | État | Solution |
|-----------|------|---------|
| **Faible** | Données locales d'une page | State local du composant |
| **Modéré** | Données partagées entre quelques composants | Parent comme source + EventCallback |
| **Élevé** | État complexe global | Service singleton + notifications |
| **Très élevé** | État très complexe | Framework de state management |

### Service de State Management simple

La plupart des applications Blazor WebAssembly peuvent utiliser un service partagé:

```
AppState Service:
- Stocke les données globales
- Fournit des méthodes pour modifier l'état
- Notifie les subscribers quand l'état change

Exemple:
CurrentUser, ShoppingCart, Theme, Notifications
```

---

### 🎯 localStorage - Persistance des Données (RecipeHub)

**Qu'est-ce que localStorage?**

localStorage est un API du navigateur qui permet de stocker des données persistantes localement. Les données restent même après la fermeture du navigateur.

**Différences: localStorage vs Service (AppState)**

| Aspect | localStorage | Service (Mémoire) |
|--------|-------------|-------------------|
| **Stockage** | Disque local du navigateur | RAM (mémoire navigateur) |
| **Persistance** | Après fermeture navigateur | Perdue au refresh |
| **Capacité** | ~5-10MB | Illimitée (RAM) |
| **Performance** | Plus lent (I/O disque) | Très rapide (mémoire) |
| **Sécurité** | Non chiffré (lisible en plain text) | Pas accessible via DevTools |
| **Entre onglets** | Oui (partagé) | Non (isolé par onglet) |

**Usage dans RecipeHub**

RecipeHub utilise localStorage pour **stocker le profil utilisateur** après authentification Auth0:

```
Login Flow:
1. User clique Login
2. Auth0 retourne ID token avec profil (Name, Email, Picture)
3. Profil STOCKÉ dans localStorage (rapide, persistant)
4. UserId créé/mappé en BD (MySQL) pour lier favoris/contacts
5. App accède au localStorage, PAS à la BD pour le profil

Avantages:
✅ Performance: Accès instantané au profil (pas de requête DB)
✅ Offline: Profil accessible même hors ligne
✅ Moins de requêtes serveur
✅ Sécurité: Données sensibles gérées par Auth0 (tokens)
```

**Données dans localStorage (RecipeHub)**

```json
{
  "user:profile": {
    "name": "Jean Dupont",
    "email": "jean@example.com",
    "picture": "https://...",
    "sub": "auth0|123456"
  },
  "user:theme": "dark",
  "user:token": "eyJhbGciOiJIUzI1NiIs...",
  "favorites:list": "[recipe-001, recipe-002, ...]"
}
```

**Manipulation de localStorage en Blazor**

```
Blazor n'a pas d'accès direct à localStorage (c'est une API JavaScript)
Solution: Utiliser Interop JavaScript

Service: LocalStorageService.cs
- GetUserProfile() → Lit localStorage
- SetUserProfile(profile) → Écrit localStorage
- ClearUserData() → Vide les données utilisateur
- GetTheme() → Lit le thème
- SetTheme(theme) → Écrit le thème
```

**Quand utiliser localStorage vs Service?**

| Données | Stockage | Raison |
|---------|----------|--------|
| **Profil utilisateur** | localStorage | Persistance + Performance |
| **Thème (dark/light)** | localStorage | Persistance entre sessions |
| **Tokens Auth** | localStorage | Récupération après refresh |
| **Favoris (cache)** | localStorage | Accès rapide |
| **État page courante** | Service (RAM) | Temporaire, pas besoin persistance |
| **Notifications** | Service (RAM) | Affichage temporaire |
| **Cart/Panier** | localStorage | Persistance entre sessions |

⚠️ **Important**: localStorage est accessible via DevTools! Ne jamais stocker:
- Mots de passe (stocker tokens Auth0 oui, mais pas mots de passe)
- Données très sensibles (SSN, numéro carte, etc.)
- Données personnelles confidentielles

---

## 11. Communication Backend {#communication-backend}

### Qu'est-ce que la communication backend?

En Blazor WebAssembly, le code C# s'exécute côté client. Pour accéder aux données (base de données, fichiers serveur, services externes), le client doit communiquer avec un serveur backend via une API.

```
FLUX DE COMMUNICATION:

┌─────────────────┐
│  NAVIGATEUR     │
│                 │
│  Blazor WASM    │────────────┐
│  Runtime .NET   │            │
│  Code C#        │            │
│                 │            │
└─────────────────┘            │
                                │
                    HTTP Request│
                                │
                                ↓
                    ┌────────────────────┐
                    │   SERVEUR BACKEND  │
                    │                    │
                    │   API REST / gRPC  │
                    │   Base de données  │
                    │   Fichiers         │
                    │                    │
                    └────────────────────┘
                                ↑
                    HTTP Response│
                                │
                    ┌───────────┘
                    │
                    ↓
┌─────────────────┐
│  NAVIGATEUR     │
│  Réponse reçue  │
│  Données mises à jour
└─────────────────┘
```

### HTTP Client Blazor

Blazor fournit `HttpClient` pour faire des requêtes HTTP.

**Injection de HttpClient**
```
@inject HttpClient http
```

**Disponibilité**
- `HttpClient` est enregistré automatiquement en DI
- Pré-configuré avec l'adresse de base du serveur

### Types de requêtes HTTP

| Méthode | Utilisation | Opération |
|---------|------------|-----------|
| **GET** | Récupérer des données | Lire |
| **POST** | Créer une nouvelle ressource | Créer |
| **PUT** | Remplacer une ressource entière | Remplacer |
| **PATCH** | Modifier partiellement une ressource | Modifier |
| **DELETE** | Supprimer une ressource | Supprimer |

### Exemples d'opérations

**GET - Récupérer une liste**
```
URL: GET /api/products
Récupère tous les produits
Réponse: Liste de produits
```

**GET avec ID - Récupérer un élément**
```
URL: GET /api/products/123
Récupère le produit avec ID 123
Réponse: Un produit
```

**POST - Créer une nouvelle ressource**
```
URL: POST /api/products
Corps: Données du nouveau produit
Réponse: Le produit créé avec son ID
```

**PUT - Remplacer une ressource**
```
URL: PUT /api/products/123
Corps: Nouvelles données complètes du produit
Réponse: Le produit mis à jour
```

**DELETE - Supprimer une ressource**
```
URL: DELETE /api/products/123
Réponse: Succès (200 OK) ou erreur
```

### Cycle d'une requête HTTP

```
┌───────────────────────────────────┐
│  1. PRÉPARATION                   │
│  - Créer l'URL                    │
│  - Préparer les données (si POST) │
│  - Configurer les headers         │
└───────────────────────────────────┘
             ↓
┌───────────────────────────────────┐
│  2. ENVOI                         │
│  - Requête HTTP au serveur        │
│  - Attendre la réponse            │
└───────────────────────────────────┘
             ↓
┌───────────────────────────────────┐
│  3. TRAITEMENT DE LA RÉPONSE      │
│  - Vérifier le statut (200, 404...) │
│  - Parser les données             │
│  - Gérer les erreurs              │
└───────────────────────────────────┘
             ↓
┌───────────────────────────────────┐
│  4. MISE À JOUR                   │
│  - Mettre à jour les variables    │
│  - Re-rendre le composant         │
│  - Afficher les données           │
└───────────────────────────────────┘
```

### Codes de statut HTTP

| Code | Signification | Signification |
|------|--------------|--------------|
| 200 | OK | Succès |
| 201 | Created | Création réussie |
| 204 | No Content | Succès sans contenu |
| 400 | Bad Request | Requête invalide |
| 401 | Unauthorized | Non authentifié |
| 403 | Forbidden | Pas de permission |
| 404 | Not Found | Ressource non trouvée |
| 500 | Internal Server Error | Erreur serveur |

### Gestion des erreurs

Les requêtes HTTP peuvent échouer pour plusieurs raisons:

| Raison | Exemple | Gestion |
|--------|---------|---------|
| **Réseau indisponible** | Pas d'internet | Afficher un message |
| **Serveur indisponible** | Serveur down | Retry ou erreur |
| **Erreur client** | URL invalide | Vérifier les paramètres |
| **Erreur serveur** | Exception en DB | Afficher le message d'erreur |
| **Dépassement temps** | Requête trop longue | Timeout et retry |

### Sérialisation JSON

Blazor convertit automatiquement entre C# et JSON:

```
┌─────────────────────────────────────┐
│  C# Object                          │
│  class Product { int id; string name; }│
│  instance { id: 1, name: "Laptop" } │
└─────────────────────────────────────┘
             ↓ Sérialisation
┌─────────────────────────────────────┐
│  JSON                               │
│  {"id": 1, "name": "Laptop"}        │
└─────────────────────────────────────┘
             ↓ Envoi HTTP
        API Serveur
             ↓ Réception
┌─────────────────────────────────────┐
│  JSON                               │
│  {"id": 1, "name": "Laptop"}        │
└─────────────────────────────────────┘
             ↓ Désérialisation
┌─────────────────────────────────────┐
│  C# Object                          │
│  Product { id: 1, name: "Laptop" }  │
└─────────────────────────────────────┘
```

### Service API typique

Une application Blazor WebAssembly utilise généralement un service pour encapsuler les appels API:

```
ProductService:
- GetAllProducts() → GET /api/products
- GetProduct(int id) → GET /api/products/{id}
- CreateProduct(Product p) → POST /api/products
- UpdateProduct(Product p) → PUT /api/products/{id}
- DeleteProduct(int id) → DELETE /api/products/{id}
```

Ce service est injecté dans les composants qui en ont besoin.

### Requêtes asynchrones

Les requêtes HTTP sont toujours asynchrones en Blazor. Le composant n'attend pas la réponse (ce qui bloquerait l'UI), mais continue à afficher le composant tandis que la requête se fait en arrière-plan.

```
Timeline:

T0: Utilisateur clique "Charger données"
    → Méthode HandleLoad() appelée
T1: Requête HTTP lancée
    → API contactée
T2-T5: Attente de la réponse
    → UI reste interactive
T6: Réponse reçue
    → Données mises à jour
T7: Composant re-rendu
    → Données affichées
```

---

## 12. AuthorizeView et Contrôle d'Accès {#authorize-view}

### Qu'est-ce que l'authentification et l'autorisation?

**Authentification**
Vérifier QUI vous êtes. Exemple: login/password

**Autorisation**
Vérifier QUOI vous pouvez faire. Exemple: peut lire vs peut écrire

```
┌─────────────────────────────────────┐
│     AUTHENTIFICATION                │
├─────────────────────────────────────┤
│                                     │
│  "Êtes-vous bien Alice?"            │
│  ← Oui, je suis Alice (login+pwd)   │
│  → Authentifié ✓                    │
│                                     │
│  Résultat: Login successful         │
│                                     │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│     AUTORISATION                    │
├─────────────────────────────────────┤
│                                     │
│  "Alice, pouvez-vous supprimer?"    │
│  ← Non, je suis lecteur             │
│  → Non autorisé ✗                   │
│                                     │
│  Résultat: Access denied            │
│                                     │
└─────────────────────────────────────┘
```

### Rôles et permissions

Un utilisateur peut avoir plusieurs rôles, et chaque rôle a des permissions:

```
┌──────────────────────────────────────┐
│         UTILISATEUR: Alice           │
│         RÔLES: Admin, Editor         │
└──────────────────────────────────────┘
             ↓
┌──────────────────────────────────────┐
│  RÔLE: Admin                         │
│  Permissions:                        │
│  - Créer                            │
│  - Lire                             │
│  - Modifier                         │
│  - Supprimer                        │
│  - Gérer les utilisateurs           │
└──────────────────────────────────────┘

┌──────────────────────────────────────┐
│  RÔLE: Editor                        │
│  Permissions:                        │
│  - Créer                            │
│  - Lire                             │
│  - Modifier son propre contenu      │
└──────────────────────────────────────┘
```

### AuthorizeView

`AuthorizeView` est un composant Blazor qui affiche ou masque du contenu basé sur l'authentification/autorisation.

**Utilité**
- Afficher du contenu seulement si l'utilisateur est connecté
- Afficher du contenu seulement si l'utilisateur a un rôle spécifique
- Afficher un message "non autorisé" si l'accès est refusé

### Cas d'utilisation d'AuthorizeView

**Cas 1: Afficher seulement si authentifié**
```
<AuthorizeView>
    <h1>Tableau de bord privé</h1>
</AuthorizeView>
```

Si l'utilisateur est connecté → affiche "Tableau de bord privé"
Si l'utilisateur n'est pas connecté → n'affiche rien

**Cas 2: Afficher un message si pas authentifié**
```
<AuthorizeView>
    <Authorized>
        Contenu pour les utilisateurs connectés
    </Authorized>
    <NotAuthorized>
        Veuillez vous connecter
    </NotAuthorized>
</AuthorizeView>
```

**Cas 3: Contrôle par rôle**
```
<AuthorizeView Roles="Admin">
    Contenu pour les Admins seulement
</AuthorizeView>

<AuthorizeView Roles="Admin, Editor">
    Contenu pour les Admins ET Editors
</AuthorizeView>
```

**Cas 4: Contenu conditionnel avec fallback**
```
<AuthorizeView Policy="IsPremium">
    <Authorized>
        Fonctionnalité premium
    </Authorized>
    <NotAuthorized>
        Devenez premium pour accéder
    </NotAuthorized>
</AuthorizeView>
```

### Cas Authorizing

Parfois, le statut d'autorisation n'est pas encore connu (en attente de vérification):

```
<AuthorizeView>
    <Authorized>
        Utilisateur connecté
    </Authorized>
    <Authorizing>
        Vérification en cours...
    </Authorizing>
    <NotAuthorized>
        Non connecté
    </NotAuthorized>
</AuthorizeView>
```

### Flux d'authentification Blazor WebAssembly

```
┌────────────────────────────────────┐
│  1. DÉMARRAGE DE L'APP             │
│  - App.razor charge                │
│  - AuthenticationStateProvider     │
│    récupère le statut actuel       │
└────────────────────────────────────┘
             ↓
┌────────────────────────────────────┐
│  2. VÉRIFICATION D'AUTHENTIFICATION │
│  - Chercher le token dans         │
│    localStorage ou sessionStorage  │
│  - Envoyer au serveur              │
│  - Serveur valide le token         │
└────────────────────────────────────┘
             ↓
┌────────────────────────────────────┐
│  3. MISE À JOUR DE L'ÉTAT          │
│  - Utilisateur authentifié ✓       │
│  - Ou pas authentifié ✗            │
│  - Récupérer les rôles/claims      │
└────────────────────────────────────┘
             ↓
┌────────────────────────────────────┐
│  4. COMPOSANTS RENDU               │
│  - AuthorizeView utilise l'état    │
│  - Affiche le contenu approprié    │
│  - User disponible dans les composants
└────────────────────────────────────┘
```

### AuthenticationStateProvider

C'est un service qui gère le statut d'authentification actuel.

**Fonctionnalité**
- Fournit le statut d'authentification
- Fournit les informations de l'utilisateur (ID, nom, rôles)
- Notifie quand l'authentification change

**Utilisation**
```
@inject AuthenticationStateProvider authStateProvider

Récupère le statut actuel:
var authState = await authStateProvider.GetAuthenticationStateAsync()

Vérifie si authentifié:
var user = authState.User
bool isAuthenticated = user.Identity?.IsAuthenticated ?? false
```

### Sécurisation des routes

On peut protéger des pages entières pour que seulement les utilisateurs authentifiés y accèdent:

```
@page "/admin"
@attribute [Authorize]
@attribute [Authorize(Roles = "Admin")]

Page visible seulement pour les Admins authentifiés
```

### Attribut @attribute

L'attribut `@attribute` applique des restrictions au composant/page:

```
[Authorize]           → Authentification requise
[Authorize(Roles = "Admin")]  → Rôle Admin requis
[AllowAnonymous]      → Accessible à tous
```

### Flux login/logout

```
┌────────────────┐
│  LOGIN PAGE    │
├────────────────┤
│ Utilisateur    │
│ saisit ID/pwd  │
│ Clique login   │
└────────────────┘
        ↓
┌────────────────────────────────┐
│  1. ENVOYER AU SERVEUR         │
│  POST /api/auth/login          │
│  Body: {email, password}       │
└────────────────────────────────┘
        ↓
┌────────────────────────────────┐
│  2. SERVEUR VÉRIFIE            │
│  Email/password valides?       │
│  Oui → génère token JWT        │
│  Non → erreur 401              │
└────────────────────────────────┘
        ↓
┌────────────────────────────────┐
│  3. RETOUR TOKEN               │
│  Réponse: {token, user}        │
└────────────────────────────────┘
        ↓
┌────────────────────────────────┐
│  4. STOCKAGE LOCAL             │
│  Sauvegarder le token dans     │
│  localStorage ou sessionStorage│
└────────────────────────────────┘
        ↓
┌────────────────────────────────┐
│  5. MISE À JOUR ÉTAT           │
│  Notifier AuthenticationState  │
│  Provider de la connexion      │
│  Utilisateur maintenant "autentifié"
└────────────────────────────────┘
        ↓
┌────────────────────────────────┐
│  6. REDIRECTION                │
│  Aller vers /dashboard         │
│  ou page privée                │
└────────────────────────────────┘

LOGOUT:
        ↓
┌────────────────────────────────┐
│  1. SUPPRIMER LOCALEMENT       │
│  Retirer le token du storage   │
└────────────────────────────────┘
        ↓
┌────────────────────────────────┐
│  2. NOTIFICATION SERVEUR       │
│  POST /api/auth/logout (optionnel)
└────────────────────────────────┘
        ↓
┌────────────────────────────────┐
│  3. MISE À JOUR ÉTAT           │
│  Notifier: Utilisateur déconnecté
└────────────────────────────────┘
        ↓
┌────────────────────────────────┐
│  4. REDIRECTION                │
│  Aller vers /login             │
└────────────────────────────────┘
```

### Claims et identité

Un utilisateur authentifié a des **claims** (affirmations) à son sujet:

| Claim | Exemple |
|-------|---------|
| `sub` (Subject/ID) | "12345" |
| `name` | "Alice" |
| `email` | "alice@example.com" |
| `role` | "Admin", "User" |
| `permission` | "read", "write" |

Ces claims permettent de déterminer les permissions et rôles.

### Protéger les API backend

L'authentification ne s'arrête pas au frontend:

```
Client Blazor            Backend API
     ↓                        ↓
GET /api/products     (sans token)
     → API rejette ✗

GET /api/products     (token en header)
     → API accepte ✓
     → Retourne les données
```

L'API backend doit aussi vérifier l'authentification!

---

## Résumé des concepts clés

### Architecture générale d'une application Blazor WebAssembly

```
┌─────────────────────────────────────────────────────┐
│           NAVIGATEUR (Exécution locale)             │
├─────────────────────────────────────────────────────┤
│                                                     │
│  App.razor (composant racine)                      │
│  ├── Router (gère le routage)                      │
│  ├── Pages/ (composants avec @page)               │
│  └── Components/ (composants réutilisables)       │
│                                                     │
│  Lifecycle des composants:                         │
│  SetParametersAsync → OnInitialized → Render       │
│  → OnAfterRender → [Événements] → StateChanged    │
│  → Re-render                                       │
│                                                     │
│  Binding des données: one-way (@) et two-way (@bind)
│  Événements: @onclick, @onchange, @onsubmit       │
│                                                     │
│  State Management:                                 │
│  State local → State partagé (parent) → Global    │
│                                                     │
│  DI Container:                                     │
│  @inject ServiceType variable                     │
│  Services: Transient / Scoped / Singleton        │
│                                                     │
└─────────────────────────────────────────────────────┘
                        ↕ HTTP
         ┌──────────────────────────────────┐
         │     SERVEUR BACKEND (API REST)   │
         │                                   │
         │  Controllers / Endpoints          │
         │  GET/POST/PUT/DELETE              │
         │  Base de données                  │
         │  Services métier                  │
         └──────────────────────────────────┘
```

### Checklist d'apprentissage Blazor WebAssembly

- [ ] Comprendre WebAssembly et son avantage
- [ ] Connaître les différences Server vs WebAssembly
- [ ] Créer et utiliser des composants
- [ ] Utiliser le binding deux-directionnels
- [ ] Traiter les événements utilisateur
- [ ] Utiliser le lifecycle des composants
- [ ] Injecter et utiliser les services
- [ ] Mettre en place le routage et la navigation
- [ ] Gérer l'état partagé
- [ ] Communiquer avec un backend via HTTP
- [ ] Implémenter l'authentification et l'autorisation

---

## Pour aller plus loin

### Points avancés (non couverts ici)

- Composants génériques (`@typeparam`)
- Render fragments et contenu personnalisé
- Interopérabilité JavaScript (JS Interop)
- Streaming rendu
- Prerendering
- Progressive Web Apps (PWA)
- Gestion d'état avancée (Redux, Fluxor)
- Testing unitaire
- Déploiement en production

### Ressources recommandées

- Documentation officielle Microsoft Blazor
- Tutoriels interactifs sur Microsoft Learn
- Communauté Blazor (forums, Discord)
- Projets open-source Blazor
- Articles techniques et blogs

---

Fin du guide.
