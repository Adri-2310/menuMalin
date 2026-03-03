# 📊 Guide des Performance Tests - MenuMalin

## 🚀 Exécution des Tests

### **Installation (une fois)**
```bash
# Ajouter BenchmarkDotNet
dotnet add menuMalin.Tests.Server package BenchmarkDotNet

# Compiler en Release (IMPORTANT pour résultats valides!)
dotnet build -c Release
```

### **Exécution Complète**
```bash
# Tous les tests de performance
cd menuMalin.Tests.Server
dotnet run -c Release

# Ou spécifiement:
dotnet run -c Release -- --filter ServiceMealDBPerformanceTests
dotnet run -c Release -- --filter ServiceRecetteUtilisateurPerformanceTests
dotnet run -c Release -- --filter RepositoryPerformanceTests
```

### **Résultats Générés**
```
BenchmarkDotNet.Artifacts/
├── results/
│   ├── ServiceMealDBPerformanceTests-*.html
│   ├── ServiceRecetteUtilisateurPerformanceTests-*.html
│   └── RepositoryPerformanceTests-*.html
├── logs/
└── exporters/
```

---

## 📈 **Tests Inclus**

### **ServiceMealDB Performance (HTTP & Retry)**

#### PERF 1: API Success (Baseline)
```
Benchmark: GetRandomAsync - API Success
Mesure: Appel API simple sans erreur
Baseline: < 5ms
Cible: API response + JSON deserialization
```

#### PERF 2: Retry Overhead
```
Benchmark: GetRandomAsync - With Retry (1 fail)
Mesure: 1 timeout suivi de succès
Baseline: 2-3x plus lent que succès direct
Impact: Validé que retry mechanism n'a pas d'overhead catastrophique
```

#### PERF 3: Large Result Set
```
Benchmark: SearchByNameAsync - 50 Results
Mesure: Deserialization de 50 résultats
Baseline: < 10ms même avec beaucoup de données
Impact: Vérifier scalabilité de la deserialization
```

#### PERF 4: Memory Allocations
```
Benchmark: GetRandomAsync - Memory Allocations
Mesure: Allocations pour 10 appels successifs
Baseline: < 5KB par appel
Impact: Détecter fuites mémoire ou allocations excessives
```

#### PERF 5: JSON Deserialization Impact
```
Benchmark: JSON Deserialization
Mesure: Coût isolé de deserialization
Baseline: < 2ms pour objet simple
Impact: Identifier bottleneck dans parsing JSON
```

---

### **Repository Performance (BD InMemory)**

#### PERF 6: Large Data Retrieval
```
Benchmark: GetByUserIdAsync - 100 Records
Mesure: Récupération de 100 recettes
Baseline: < 5ms
Impact: Performance des requêtes LINQ
```

#### PERF 7: Filtering
```
Benchmark: GetPublicAsync - Filter Public
Mesure: Filtrer moitié de 100 recettes
Baseline: < 5ms
Impact: Coût du filtrage LINQ (IsPublic == true)
```

#### PERF 8: Insert
```
Benchmark: AddAsync - Single Insert
Mesure: Insertion simple
Baseline: < 2ms
Impact: Overhead du changement tracking EF Core
```

---

### **ServiceRecetteUtilisateur Performance (Validation & Sécurité)**

#### PERF 9: Validation Overhead
```
Benchmark: CreateAsync - Validation Overhead
Mesure: Coût de la validation avant DB
Baseline: < 1ms (validation seule, sans DB)
Impact: Vérifier que validation n'est pas bottleneck
```

#### PERF 10: JSON Serialization
```
Benchmark: CreateAsync - 50 Ingredients
Mesure: Sérialisation de 50 ingrédients
Baseline: < 2ms même avec beaucoup
Impact: Scalabilité de la sérialisation JSON
```

#### PERF 11: Owner Check
```
Benchmark: UpdateAsync - Owner Check
Mesure: Coût de la vérification du propriétaire
Baseline: < 1ms
Impact: Performance des vérifications de sécurité
```

#### PERF 12: Image Deletion
```
Benchmark: DeleteAsync - With Image Deletion
Mesure: Vérification propriétaire + suppression image
Baseline: < 5ms
Impact: I/O file deletion overhead
```

#### PERF 13: Large Dataset DTO Conversion
```
Benchmark: GetMyRecipesAsync - 100 Recipes
Mesure: Désérialisation + conversion pour 100 recettes
Baseline: < 10ms
Impact: Performance de mapping données
```

#### PERF 14: SSRF Validation
```
Benchmark: CreateAsync - SSRF Validation
Mesure: Coût de la validation SSRF (URL parsing + IP checking)
Baseline: < 1ms
Impact: Sécurité sans perf dégradée
```

#### PERF 15: DTO Mapping
```
Benchmark: DTO Mapping - 100 Conversions
Mesure: Allocations lors de 100 mappages
Baseline: < 5KB par conversion
Impact: Mémoire utilisée pour mapping
```

---

## 🔍 **Interprétation des Résultats**

### **Exemple de Résultat BenchmarkDotNet**
```
| Method          | Mean      | StdDev    | Gen 0 | Gen 1 | Gen 2 | Allocated |
|-----------------|-----------|-----------|-------|-------|-------|-----------|
| GetRandomAsync  |   2.34 ms |   0.12 ms |    24 |     0 |     0 |   7.5 KB  |
```

**Colonnes:**
- **Mean**: Temps moyen (cible: < baseline)
- **StdDev**: Déviation standard (< 10% = bon)
- **Gen 0/1/2**: Garbage collections (0 = mieux)
- **Allocated**: Mémoire allouée (cible: petit et stable)

### **Interprétation**
```
✅ BON:
- Mean < baseline
- StdDev < 10%
- Gen 0 stable
- Allocated < 10KB

⚠️ À INVESTIGUER:
- Mean croît de 20%+ vs version précédente
- StdDev > 20% (instabilité)
- Gen 2 collections excessives
- Allocated croît rapidement

❌ PROBLÈME:
- Mean > 100ms
- Gen 2 collections à chaque run
- Allocated > 100KB
- Crash out-of-memory
```

---

## 📊 **Comparaison Entre Runs**

### **Sauvegarder les Résultats**
```bash
# Automatique - résultats dans BenchmarkDotNet.Artifacts/

# Exporter en JSON
dotnet run -c Release -- --exporters json

# Exporter en CSV
dotnet run -c Release -- --exporters csv
```

### **Comparer Deux Versions**
```bash
# Version A: Run A
dotnet run -c Release > results_v1.txt

# Version B: (après modifications)
dotnet run -c Release > results_v2.txt

# Comparer manuellement
diff results_v1.txt results_v2.txt
```

---

## 🎯 **Baselines & Objectifs**

| Benchmark | Baseline | Acceptable | Alert |
|-----------|----------|-----------|-------|
| API Success | < 5ms | < 10ms | > 20ms |
| With Retry | 2-3x baseline | < 20ms | > 30ms |
| 50 Results | < 10ms | < 15ms | > 30ms |
| Validation | < 1ms | < 2ms | > 5ms |
| 50 Ingredients | < 2ms | < 5ms | > 10ms |
| Owner Check | < 1ms | < 2ms | > 5ms |
| Image Delete | < 5ms | < 10ms | > 20ms |
| 100 Recipes | < 10ms | < 20ms | > 50ms |
| SSRF Validation | < 1ms | < 2ms | > 5ms |
| DTO Mapping | < 5KB | < 10KB | > 50KB |

---

## 🔧 **Troubleshooting**

### **Les résultats fluctuent beaucoup**
```
Cause: CPU trop chargé, thermal throttling
Solution:
1. Fermer les autres applis
2. Augmenter warmupCount: 5
3. Augmenter targetCount: 10
4. Utiliser ThreadingDiagnoser
```

### **Allocation "allocated" très haute**
```
Cause: Allocations mémoire excessives
Solution:
1. Vérifier string allocations
2. Utiliser StringBuilder pour concatenations
3. Pooling objects (ArrayPool, etc)
4. Lazy initialization
```

### **GC Gen 2 Collections**
```
Cause: Objets vivant longtemps
Solution:
1. Réduire lifetime objects
2. Utiliser using/IDisposable
3. Vérifier memory leaks
4. Profiler avec dotTrace
```

---

## 📈 **Progression Monitoring**

### **CI/CD Integration**
```yaml
# Dans GitHub Actions / GitLab CI:
- Run: dotnet run -c Release -- --threshold 105%
# Alert si perf dégradée > 5%
```

### **Alerting**
```bash
# Alert si > 10ms
dotnet run -c Release -- --filter ServiceMealDBPerformanceTests --threshold 110%
```

---

## 🚀 **Optimisation Tips**

### **Si Timeout (Retry) est lent**
1. Réduire délai initial (200ms → 100ms)
2. Utiliser exponential backoff avec cap
3. Async/await au lieu de Task.Wait()

### **Si Deserialization est slow**
1. Cache serializers
2. Utiliser source generators (System.Text.Json)
3. Streaming JSON parsing pour gros volumes

### **Si DTO Mapping consomme trop mémoire**
1. Mapper directement sans DTO intermediaire
2. Lazy loading attributes
3. Reuse DTOs (object pooling)

### **Si BD Queries sont lentes**
1. Ajouter index (.HasIndex())
2. Eager loading (.Include())
3. Projection (.Select())

---

## 📝 **Exemple Complet: Optimiser 'GetMyRecipesAsync'**

### **Avant**
```csharp
// Baseline: 15ms pour 100 recettes
var recipes = await _repository.GetByUserIdAsync(userId);
var dtos = recipes
    .Select(r => new RecetteUtilisateurDTO { ... })
    .ToList();
return dtos;
```

### **Profiling**
```
| Method | Mean | Gen 0 | Allocated |
|--------|------|-------|-----------|
| Before | 15ms | 50    | 250 KB    |
```

### **Optimisé**
```csharp
// Target: < 10ms, < 100KB
var recipes = await _repository.GetByUserIdAsync(userId)
    .ConfigureAwait(false);
// Lazy mapping
return recipes.AsEnumerable()
    .Select(MapToDto)  // Delegate instead of lambda
    .ToList();
```

### **Après**
```
| Method | Mean | Gen 0 | Allocated |
|--------|------|-------|-----------|
| After  | 8ms  | 15    | 85 KB     |
| Gain   | -46% | -70%  | -66%      |
```

---

## 🔗 **Ressources**

- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/)
- [.NET Performance Best Practices](https://docs.microsoft.com/en-us/dotnet/fundamentals/networking/http/performance-best-practices)
- [Profiling Tools](https://docs.microsoft.com/en-us/visualstudio/profiling/)

---

**Dernier Update**: 2026-03-03
**Version**: 1.0
