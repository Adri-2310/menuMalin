# Guide Complet Entity Framework Core et Migrations

## 1. C'est quoi Entity Framework Core ?

Entity Framework Core (EF Core) est un ORM (Object-Relational Mapping) moderne et léger développé par Microsoft pour .NET. C'est un framework qui permet aux développeurs d'interagir avec les bases de données à l'aide d'objets .NET au lieu d'écrire du SQL brut.

### Caractéristiques principales :
- **Abstraction de la base de données** : Travaillez avec des objets C# plutôt que du SQL direct
- **Support multi-bases de données** : SQL Server, PostgreSQL, MySQL, SQLite, etc.
- **Lightweight et performant** : Conçu pour être rapide et flexible
- **Async/await natif** : Support complet des opérations asynchrones
- **Migrations intégrées** : Système de versioning du schéma de base de données
- **LINQ** : Interrogation type-safe en C#

Entity Framework Core remplace l'ancien Entity Framework 6 et offre une meilleure performance, une architecture modulaire et un code plus moderne.

---

## 2. DbContext (Concept)

Le DbContext est le cœur d'Entity Framework Core. C'est la classe principale qui représente une session de travail avec la base de données.

### Rôle du DbContext :
- **Gestionnaire de connexion** : Gère la connexion à la base de données
- **Unité de travail** : Suit tous les changements effectués sur les entités
- **Traduction LINQ** : Convertit les requêtes LINQ en SQL
- **Gestion des transactions** : Contrôle les transactions avec SaveChanges()

### Responsabilités clés :
- **Configuration** : Définit comment les entités sont mappées à la base de données via OnModelCreating
- **DbSet** : Collections représentant les tables de la base de données
- **Change Tracking** : Suivi automatique des modifications apportées aux entités
- **SaveChanges** : Persiste les changements dans la base de données

### Cycle de vie du DbContext :
Le DbContext passe par plusieurs états :
1. **Création** : Instanciation du contexte
2. **Utilisation** : Ajout, modification, suppression d'entités
3. **SaveChanges** : Envoi des changements à la base de données
4. **Disposal** : Libération des ressources (connexion, transactions)

### Pattern courant :
Le DbContext doit être utilisé avec le pattern using() ou dependency injection pour garantir la libération des ressources. Chaque opération devrait créer une nouvelle instance du contexte ou utiliser une instance gérée par le conteneur DI.

---

## 3. Entities et Relationships

### Qu'est-ce qu'une Entity ?

Une Entity est une classe C# qui représente une table dans la base de données. Chaque instance de la classe représente une ligne dans la table.

### Caractéristiques des Entities :
- **Propriétés** : Correspondent aux colonnes de la table
- **Primary Key** : Identifie de manière unique chaque enregistrement
- **Navigation Properties** : Permettent d'accéder aux entités liées

### Types de données supportées :
- Types primitifs : int, string, decimal, datetime, bool, guid, etc.
- Types nullable : int?, decimal?, datetime?, etc.
- Collections : ICollection<T>, List<T> pour les relations
- Owned Types : Types imbriqués qui n'ont pas de table propre

### Conventions de nommage :
- **Primary Key** : Nommée "Id" ou "{NomClasse}Id" (convention)
- **Propriétés** : Commencent par une majuscule (PascalCase)
- **Clés étrangères** : Nommées "{NomNavigationProperty}Id"

### Types de Relationships

#### 1. One-to-Many (Un-à-Plusieurs)
Une entité parente peut avoir plusieurs enfants. Par exemple, un Client peut avoir plusieurs Commandes.
- La table enfant contient une clé étrangère vers le parent
- Navigation : Collection sur le parent, Objet sur l'enfant

#### 2. Many-to-One (Plusieurs-à-Un)
L'inverse d'une relation One-to-Many. Plusieurs Commandes appartiennent à un Client.
- Navigation : Référence à l'objet parent

#### 3. One-to-One (Un-à-Un)
Une entité est liée à une seule autre entité. Par exemple, un Utilisateur a un Profil.
- Peut être implémentée avec une clé étrangère unique
- Navigation : Objet sur les deux côtés

#### 4. Many-to-Many (Plusieurs-à-Plusieurs)
Plusieurs entités sont liées à plusieurs autres. Par exemple, des Étudiants suivent plusieurs Cours et un Cours a plusieurs Étudiants.
- Nécessite généralement une table de jointure
- Navigation : Collections sur les deux côtés
- En EF Core 5+, les relations many-to-many peuvent être définies directement

### Configuration des Relationships :
Les relationships peuvent être définies de plusieurs façons :
- **Par convention** : EF Core détecte automatiquement les relations basées sur les noms de propriétés
- **Avec les data annotations** : Attributs sur les propriétés
- **En Fluent API** : Configuration dans OnModelCreating pour plus de contrôle

### Cascade Actions :
Définissent le comportement lors de la suppression d'une entité parente :
- **Cascade** : Supprime les enfants (comportement par défaut pour les FK non-nullables)
- **Restrict** : Empêche la suppression si des enfants existent
- **SetNull** : Définit la FK à NULL (uniquement si nullable)
- **NoAction** : Aucune action automatique

---

## 4. Migrations (C'est quoi)

Les Migrations sont un système de versioning pour votre schéma de base de données. Elles permettent de tracker les changements du modèle de données et de synchroniser la base de données.

### Concept fondamental :
Une migration est un ensemble d'instructions qui décrivent comment modifier la structure de la base de données. Chaque migration représente une étape dans l'évolution du schéma.

### Problèmes résolus par les Migrations :
- **Source de vérité** : Garder le modèle C# et la base de données synchronisés
- **Historique** : Tracer tous les changements du schéma
- **Collaboration** : Plusieurs développeurs travaillant sur le même projet
- **Production** : Appliquer les changements de manière contrôlée en production
- **Rollback** : Revenir à une version antérieure si nécessaire

### Anatomie d'une Migration :

Une migration contient trois parties principales :
1. **Classe de migration** : Hérite de Migration, contient la logique
2. **Méthode Up** : Instructions pour appliquer la migration (structure la base de données)
3. **Méthode Down** : Instructions pour annuler la migration (rollback)

### Snapshot du modèle :
Un fichier snapshot est généré pour chaque migration, représentant l'état actuel du modèle au moment de la migration. Il permet à EF Core de détecter les changements futurs en comparant le modèle actuel avec le dernier snapshot.

### Détection automatique des changements :
EF Core détecte automatiquement les changements en comparant le modèle actuel avec le dernier snapshot, notamment :
- Propriétés ajoutées ou supprimées
- Changements de type de données
- Modifications des contraintes
- Changements de relationships

### State Machine des Migrations :
Chaque migration a un état :
- **Pending** : Migration créée mais non appliquée
- **Applied** : Migration appliquée à la base de données
- **Superseded** : Migration ancienne, version plus récente appliquée

---

## 5. Créer une Première Migration

### Prérequis :

Avant de créer une migration, assurez-vous que :
- Le DbContext est correctement défini
- Les entités sont définies avec les propriétés et relationships appropriées
- La connection string est configurée
- Les packages EF Core sont installés (Core + Provider spécifique)

### Processus de création :

La création d'une migration suit ces étapes :

1. **Détection des changements** : EF Core compare le modèle actuel avec le dernier snapshot
2. **Génération du code** : Crée une classe de migration avec les instructions SQL
3. **Création du snapshot** : Met à jour le fichier snapshot
4. **Organisation des fichiers** : Place les fichiers dans le dossier Migrations

### Conventions de dénomination :
Les migrations créées automatiquement reçoivent des noms descriptifs basés sur les changements :
- AddUserTable (si une table est ajoutée)
- AddPhoneNumberToUser (si une colonne est ajoutée)
- DropUnusedColumn (si une colonne est supprimée)

### Types de changements détectés :
- Ajout/suppression d'entités
- Ajout/suppression/modification de propriétés
- Changements de contraintes (unique, max length)
- Changements de relationships
- Changements de types de données

### Idempotence :
Les migrations doivent être idempotentes, c'est-à-dire qu'appliquer deux fois la même migration ne cause pas d'erreur. EF Core gère cela automatiquement.

### Migrations vides :
Il est possible de créer une migration vide et d'ajouter manuellement du code pour des opérations spéciales (seed de données, migration complexe, etc.). Cette approche offre plus de flexibilité.

---

## 6. Appliquer une Migration

### Concept :
Appliquer une migration signifie exécuter les instructions SQL générées pour mettre à jour la base de données selon la migration.

### États de la base de données :
Avant d'appliquer une migration, la base de données peut être dans plusieurs états :
- **Vierge** : Aucune table n'existe, première migration va créer l'infrastructure
- **Partiellement migrée** : Certaines migrations sont appliquées, d'autres non
- **À jour** : Toutes les migrations sont appliquées

### Tracking des migrations appliquées :
EF Core maintient une table spéciale (généralement `__EFMigrationsHistory`) qui stocke les noms des migrations appliquées. Cette table :
- Est créée automatiquement lors de la première migration
- Enregistre chaque migration appliquée
- Permet à EF Core de savoir quelles migrations appliquer

### Processus d'application :
1. **Lecture du historique** : EF Core lit les migrations déjà appliquées
2. **Identification des migrations pending** : Compare avec les migrations du projet
3. **Exécution des migrations** : Exécute les méthodes Up dans l'ordre
4. **Mise à jour du historique** : Enregistre les migrations appliquées
5. **Validation** : Vérifie que la base de données est à jour

### Application partielle :
Il est possible d'appliquer seulement certaines migrations jusqu'à un point spécifique, ce qui permet :
- De revenir à une version antérieure (rollback)
- De sauter des migrations
- De synchroniser partiellement

### Validation après application :
Après l'application, il est recommandé de vérifier que :
- Les tables sont créées correctement
- Les colonnes ont le bon type de données
- Les contraintes sont en place
- Les indexes sont créés
- Les relationships sont correctes

---

## 7. Migrations en Production

### Différences entre développement et production :

En production, les migrations doivent être gérées différemment pour éviter les problèmes :
- **Downtime** : Les modifications peuvent affecter l'application
- **Performance** : Les migrations sur grandes tables peuvent être lentes
- **Rollback** : Plus difficile en production
- **Données** : Les données existantes ne doivent pas être perdues

### Stratégies de déploiement en production :

#### 1. Migration hors ligne
L'application est arrêtée pendant la migration. C'est le plus simple mais cause un downtime.

#### 2. Migration avec compatibilité rétroactive
Les modifications sont compatibles avec l'ancienne et la nouvelle version de l'application :
- Ajout de colonnes nullables
- Ajout de tables
- Modification graduelle du schéma

#### 3. Déploiement bleu-vert
Deux environnements identiques sont maintenus :
- Version actuelle (bleu) traite le trafic
- Nouvelle version (vert) est préparée
- Basculement du trafic après validation

#### 4. Déploiement en canarie
La nouvelle version est déployée graduellement :
- Petit pourcentage du trafic vers la nouvelle version
- Augmentation progressive si tout fonctionne
- Rollback rapide si problèmes

### Préparation des migrations pour la production :

Avant de déployer :
- **Tester complètement** : Appliquer sur un clone de la base de données de production
- **Mesurer la performance** : Identifier les migrations lentes
- **Plan de rollback** : Avoir une stratégie claire en cas d'erreur
- **Communication** : Informer les utilisateurs du downtime prévu (si applicable)
- **Sauvegarde** : Effectuer une sauvegarde complète avant migration
- **Validation des données** : S'assurer que les données seront correctes après migration

### Outils de gestion en production :

Les outils disponibles pour gérer les migrations :
- **Scripts SQL générés** : Exécuter manuellement pour plus de contrôle
- **Infrastructure as Code** : Automatiser via des pipelines CI/CD
- **Monitoring** : Observer l'impact de la migration sur les performances
- **Logging** : Enregistrer les migrations appliquées avec timestamps

### Monitoring et alertes :

Pendant l'application en production :
- Surveiller la charge de la base de données
- Suivre les erreurs d'application
- Vérifier les temps de réponse
- Alerter l'équipe en cas de problème

---

## 8. Rollback d'une Migration

### Concept du Rollback :
Le rollback est le processus d'annulation d'une ou plusieurs migrations appliquées, revenant à un état antérieur de la base de données.

### Quand faire un rollback :

Un rollback est nécessaire quand :
- Une migration cause des erreurs
- Les performances sont dégradées
- Les données sont corrompues après migration
- Une mauvaise migration a été appliquée
- Une décision a changé et une fonctionnalité doit être annulée

### Problèmes avec le rollback :

Le rollback n'est pas toujours possible :
- **Perte de données** : Si une colonne est supprimée, les données sont perdues (sauf restauration)
- **Dépendances** : D'autres migrations peuvent dépendre de la migration annulée
- **Migrations complexes** : Pas de simple inverse (ex : fusion de deux tables)
- **Script Down incomplet** : La méthode Down peut ne pas restaurer complètement

### Stratégies de rollback :

#### 1. Rollback complet
Annuler toutes les migrations jusqu'à un point de départ.
- Revient l'application à une version stable
- Peut causer une perte de données
- Plus simple à exécuter

#### 2. Rollback sélectif
Annuler des migrations spécifiques en maintenant les autres.
- Permet plus de granularité
- Plus complexe à gérer
- Risque d'incohérence de schéma

#### 3. Forward-only strategy
Au lieu de rollback, créer une nouvelle migration pour corriger.
- Migration n'est jamais annulée
- Nouvelle migration corrige l'erreur
- Préféré en production (plus sûr)

### Étapes du rollback :

1. **Identification de la cible** : Déterminer jusqu'à quelle migration revenir
2. **Exécution de Down** : Exécuter la méthode Down de la migration à annuler
3. **Mise à jour du historique** : Mettre à jour la table des migrations
4. **Validation** : Vérifier que le schéma est correct
5. **Données** : S'assurer que les données sont intactes (sinon restaurer depuis backup)

### Précautions :

Avant de rollback :
- **Sauvegarde** : Effectuer une sauvegarde complète de la base de données
- **Test** : Effectuer le rollback sur une copie de la base de données
- **Communication** : Informer l'équipe du rollback prévu
- **Plan B** : Avoir une autre stratégie en cas d'échec

### Limitations :

Le rollback a des limitations :
- **Données modifiées** : Si des données ont été modifiées pendant l'exécution de la migration
- **Données supprimées** : Si une colonne/table contenant des données a été supprimée
- **Scripts Down incomplets** : Nécessitent d'être enrichis avec logique custom

---

## 9. Naming Conventions Migrations

### Importance du nommage :

Le nommage des migrations est crucial pour :
- **Compréhension** : Comprendre rapidement ce qu'une migration fait
- **Maintenance** : Trouver les migrations pertinentes
- **Collaboration** : Communiquer avec l'équipe sur les changements
- **Debugging** : Identifier les problèmes

### Convention de nommage automatique :

EF Core génère automatiquement des noms descriptifs :
- Format : `[Timestamp][Description]`
- Timestamp : Permet d'identifier l'ordre chronologique
- Description : Décrit les changements

### Exemples de noms générés :
- `20240115100530_AddUserTable.cs` : Ajout d'une table User
- `20240115100630_AddPhoneNumberToUser.cs` : Ajout d'une colonne PhoneNumber à User
- `20240115100730_CreateOrderProductRelationship.cs` : Création d'une relation many-to-many

### Nommage personnalisé :

Il est possible de créer une migration avec un nom personnalisé :
- Le nom doit être descriptif en anglais (convention industrielle)
- Utiliser des verbes : Add, Remove, Rename, Alter, Create, Drop, Update
- Inclure les entités concernées
- Éviter les caractères spéciaux

### Bonnes pratiques :

Pour un bon nommage :

1. **Être descriptif** : Le nom doit clairement indiquer le changement
   - BON : AddCreatedAtTimestampToUser
   - MAUVAIS : UpdateUser

2. **Être court** : Éviter les noms trop longs
   - BON : AddUniqueConstraintToEmail
   - MAUVAIS : AddUniqueConstraintToUserEmailColumnBecauseEmailsDuplicated

3. **Utiliser l'anglais** : Convention industrielle pour la maintenance future

4. **Être cohérent** : Maintenir la même convention à travers toutes les migrations
   - Toujours Add, Remove, Alter (pas AddNew, Delete, Change)

5. **Refléter le changement** : Le nom doit correspondre exactement au code
   - Si plusieurs changements : Créer plusieurs migrations ou nommer pour le changement principal

### Préfixes courants :

- **Add** : Ajouter une table, colonne, constraint, index
- **Remove** : Supprimer une table, colonne, constraint, index
- **Rename** : Renommer une table, colonne
- **Alter** : Modifier une propriété existante (type, nullable, etc.)
- **Create** : Créer une structure complexe (relationship, view)
- **Drop** : Supprimer une structure (généralement synonyme de Remove)
- **Update** : Mettre à jour des données (seed, transformation)

### Nommage des migrations vides :

Pour les migrations manuelles :
- Garder la même convention de nommage
- Être clair sur le but
- Exemples : `20240115100830_SeedInitialData.cs`, `20240115100930_AddStoredProcedure.cs`

### Versioning dans les noms :

Certains équipes incluent un numéro de version :
- Format : `v1_AddUserTable.cs`, `v2_AddEmailToUser.cs`
- Moins recommandé car le timestamp suffit déjà

### Documentation des migrations :

Il est recommandé de documenter :
- Raison du changement
- Impact sur les données existantes
- Problèmes connus ou limitations
- Effort de rollback
- Performance impact

Cette documentation peut être ajoutée en commentaires dans la classe de migration.

---

## 10. Versioning et Branching

### Stratégie de versioning :

Le versioning des migrations gère l'évolution du schéma de base de données à travers les versions de l'application.

### Versions de l'application :

Les migrations sont étroitement liées aux versions de l'application :
- **v1.0** : Version initiale avec premier schéma
- **v1.1** : Patch avec migrations mineures
- **v2.0** : Nouvelle version majeure avec changements importants du schéma
- **v2.1** : Évolutions supplémentaires

### Stratégie Semantic Versioning avec migrations :

**Major.Minor.Patch**
- **Major** : Changements de schéma breaking (suppression de colonnes, restructuration)
- **Minor** : Nouvelles tables, nouvelles colonnes compatibles
- **Patch** : Corrections de données, optimisations

### Branching et migrations :

#### Branches de feature
Quand plusieurs développeurs travaillent en parallèle :
- Chaque branch a ses propres migrations
- Noms uniques (timestamps garantissent l'unicité)
- Risque de conflits de migrations

#### Fusion de branches
Quand deux branches avec des migrations sont fusionnées :
- Les migrations coexistent
- Elles s'exécutent dans l'ordre du timestamp
- Possible que l'ordre d'exécution ne soit pas celui prévu (différent des deux branches)

#### Gestion des conflits de migrations :

Les conflits surviennent quand :
- Deux branches ajoutent des colonnes à la même table
- Deux branches modifient le même constraint
- L'ordre d'exécution des migrations devient ambigu

**Stratégies de résolution :**

1. **Rebasing** : Une branche rebase sur l'autre, ses migrations s'exécutent après
2. **Squashing** : Combiner les migrations en une seule
3. **Renaming** : Renommer les migrations pour clarifier l'ordre
4. **Coordination** : Discuter avec l'équipe avant de merger

### Pattern de branching :

#### Git Flow
- **main** : Code en production, migrations stables
- **develop** : Intégration, migrations en cours de développement
- **feature/xxx** : Branches de feature avec migrations locales
- **hotfix/xxx** : Corrections de production

Migrations flow :
1. Feature branch crée des migrations
2. Merge sur develop (potentiel conflit)
3. Release prepare les migrations
4. Merge sur main (production-ready)

#### GitHub Flow
- **main** : Code en production, toujours deployable
- **feature/xxx** : Branches de feature
- PR pour merger

Migrations flow :
1. Feature branch avec migrations
2. PR avec validation des migrations
3. Merge si OK
4. Deploy en production

### Migrations et déploiement continu :

Avec CI/CD :
- Les migrations sont testées automatiquement
- Validation que les migrations sont idempotentes
- Vérification qu'aucune migration n'est perdue
- Audit trail des migrations déployées

### Stratégie multi-environnements :

Migrations progressent à travers les environnements :
1. **Développement** : Migrations fréquentes et expérimentales
2. **Testing** : Migrations validées et testées
3. **Staging** : Migrations avant production
4. **Production** : Migrations stables et validées

### Gestion des versions de base de données :

La base de données peut être sur une version différente de l'application :
- **Backward compatibility** : Nouvelle version d'app avec ancienne BD
- **Forward compatibility** : Ancienne version d'app avec nouvelle BD
- **Pas garantie** : Généralement, ils doivent être en sync

### Versioning du modèle de données :

Documenter les versions du modèle :
- Version du schéma : Numéro global augmentant avec chaque migration
- Hachage du modèle : Checksum du dernier état du modèle
- Historique : Log de tous les changements

### Tag de versions Git :

Pour chaque release :
- Créer un tag Git (ex: v1.2.0)
- Tag inclut le commit avec la dernière migration
- Permet de restaurer une version spécifique avec son schéma
- Combiné avec les migrations pour reproductibilité

### Migrations non-déployables :

Parfois une migration ne peut pas être appliquée à la production :
- Migration expérimentale abandonnée
- Migration correctrice appliquée localement
- Migration de correction de bug qui n'est pas nécessaire

Ces migrations doivent être :
- Documentées
- Marquées comme obsolètes
- Éventuellement supprimées de la branche principale

---

## 11. Problèmes Courants et Solutions

### Problème 1 : Migrations en conflit après fusion de branches

**Symptômes :**
- Deux migrations avec des timestamps similaires
- L'une ou l'autre a été appliquée avant la fusion
- Erreur lors de l'application des migrations

**Causes :**
- Deux développeurs créent des migrations en parallèle
- Merged sans coordination
- L'ordre d'exécution devient ambigu

**Solutions :**
1. **Reonommer une migration** : Utiliser un timestamp plus récent
2. **Squashing** : Combiner les deux migrations en une
3. **Recréer la migration** : Supprimer l'une des migrations et recrée basée sur l'autre
4. **Communication** : Synchroniser avec l'équipe avant merge

### Problème 2 : Migration appliquée en production qui doit être annulée

**Symptômes :**
- Migration provoque des erreurs en production
- Performance dégradée
- Données corrompues

**Causes :**
- Migration pas assez testée
- Comportement inattendu en production
- Données réelles différentes du test

**Solutions :**
1. **Forward-only** : Créer une nouvelle migration pour corriger (meilleure pratique)
2. **Rollback** : Annuler la migration si possible (avec sauvegarde préalable)
3. **Hotfix** : Créer une branche hotfix avec la correction, mergée rapidement

### Problème 3 : Perte de données pendant une migration

**Symptômes :**
- Colonnes supprimées avec les données
- Données transformées incorrectement
- Pas de possibilité de récupérer les données

**Causes :**
- Migration supprime une colonne
- Migration change le type de données sans conversion
- Erreur dans la logique de migration

**Solutions :**
1. **Sauvegarde préalable** : Toujours sauvegarder avant migration en production
2. **Restoration** : Restaurer depuis la sauvegarde
3. **Colonne temporaire** : Plutôt que supprimer, archiver dans une colonne temporaire
4. **Scripts de validation** : Valider les données après migration

### Problème 4 : Migration ne s'applique pas correctement

**Symptômes :**
- Erreur SQL lors de l'application
- Migration appliquée mais table n'existe pas
- Incohérence entre le modèle et la base de données

**Causes :**
- Code SQL incorrect généré
- Migration dépendante d'une autre non appliquée
- Erreur lors de la création du snapshot

**Solutions :**
1. **Inspecter le SQL** : Examiner le SQL généré et vérifier la syntaxe
2. **Test d'abord** : Tester sur une copie de la base de données
3. **Vérifier les dépendances** : S'assurer que les migrations prérequises sont appliquées
4. **Recréer la migration** : Supprimer et recréer la migration si le code est corrompu

### Problème 5 : Deux développeurs ont créé des migrations pour le même changement

**Symptômes :**
- Deux migrations qui font pratiquement la même chose
- Erreur lors de l'application (colonne déjà existe, etc.)
- Redondance dans l'historique

**Causes :**
- Manque de communication entre développeurs
- Travail sur des branches en parallèle
- Detached branching

**Solutions :**
1. **Supprimer la redondance** : Garder une migration, supprimer l'autre
2. **Merger les changements** : Combiner les deux migrations si elles sont complémentaires
3. **Rebase** : Rebase une branche pour utiliser la migration de l'autre
4. **Processus** : Implémenter un processus de communication sur les migrations

### Problème 6 : Migration trop lente en production

**Symptômes :**
- Migration prend longtemps à s'appliquer (minutes ou heures)
- Application semble figée pendant la migration
- Timeout de base de données

**Causes :**
- Migration sur une grande table
- Index créés sans stratégie
- Allocation mémoire insuffisante
- Blocages entre migrations

**Solutions :**
1. **Batching** : Diviser la migration en plus petites unités
2. **Index** : Créer des index stratégiquement
3. **Offline** : Exécuter en mode offline si possible
4. **Async** : Utiliser l'async/await pour ne pas bloquer
5. **Monitoring** : Observer la migration et ajuster si nécessaire

### Problème 7 : Modèle et base de données désynchronisés

**Symptômes :**
- Le modèle C# ne correspond pas à la base de données
- EF Core génère des migrations non-sens
- Erreurs lors de l'interrogation de la base de données

**Causes :**
- Migrations manuelles appliquées en production
- Modifications directes du schéma sans migration
- Snapshot corrompu

**Solutions :**
1. **Add-Migration** : Générer une migration pour synchroniser
2. **Script SQL** : Générer un script SQL pour vérifier
3. **Reset** : Supprimer les migrations et recommencer (développement uniquement)
4. **Documentation** : Documenter tous les changements du schéma

### Problème 8 : Cannot drop object because it is referenced by a FOREIGN KEY constraint

**Symptômes :**
- Erreur SQL lors de la suppression d'une colonne ou table
- Migration échoue à cause d'une contrainte

**Causes :**
- Colonne ou table est référencée par une clé étrangère
- La clé étrangère n'est pas supprimée avant
- Ordre des migrations incorrect

**Solutions :**
1. **Supprimer la FK d'abord** : Migration pour supprimer la clé étrangère
2. **Puis supprimer la colonne** : Nouvelle migration pour supprimer la colonne
3. **Cascade** : Configurer la cascade delete si approprié
4. **Vérifier l'ordre** : S'assurer que l'ordre des migrations est logique

### Problème 9 : Migration crée des indexes qui ralentissent les inserts

**Symptômes :**
- Les inserts sont beaucoup plus lents après migration
- Migration ajoute des indexes
- Performance dégradée sur les opérations d'écriture

**Causes :**
- Trop d'indexes créés
- Indexes sur les colonnes fréquemment mises à jour
- Problème de statistiques

**Solutions :**
1. **Analyser les indexes** : Vérifier lesquels sont réellement nécessaires
2. **Supprimer** : Créer une migration pour supprimer les indexes inutiles
3. **Stratégie** : Créer les indexes en dehors des heures de pointe
4. **Maintenance** : Planifier la maintenance régulière des indexes

### Problème 10 : EF Core n'a pas détecté un changement du modèle

**Symptômes :**
- Changement fait au modèle mais pas détecté
- Migration vide générée
- Snapshot pas mis à jour

**Causes :**
- Le changement n'est pas reconnu comme migration
- Changement de convention au lieu de changement explicite
- Cache du contexte

**Solutions :**
1. **Migration vide** : Créer une migration vide et ajouter manuellement
2. **Recréer le DbContext** : Recommencer le contexte pour nettoyer les caches
3. **Forcer la migration** : Utiliser -Force pour ignorer l'avertissement
4. **Vérifier la détection** : S'assurer que les conventions sont correctes

### Problème 11 : Différences entre entités et la table générée

**Symptômes :**
- Propriété dans l'entité mais pas dans la table
- Colonne dans la table mais pas dans l'entité
- Comportement inattendu des données

**Causes :**
- Configuration Fluent API oubliée
- Propriété ignorée avec [NotMapped]
- Conventions mal comprises

**Solutions :**
1. **Vérifier la configuration** : S'assurer que OnModelCreating est correct
2. **Vérifier les annotations** : Chercher les [NotMapped] ou autres annotations
3. **Logs** : Activer le logging EF Core pour voir comment le modèle est interprété
4. **Documentation** : Lire la doc sur les conventions de EF Core

### Problème 12 : Migrer d'une autre version de EF ou autre ORM vers EF Core

**Symptômes :**
- Base de données existante, pas de migrations EF Core
- Schéma existant différent de ce qu'EF Core attend
- Nécessité de migrer les données

**Causes :**
- Projet utilisant EF6 ou autre solution
- Base de données existante sans ORM
- Migration complexe entre systèmes

**Solutions :**
1. **Scaffold existing DB** : Utiliser le reverse engineering pour générer le modèle
2. **Vérifier le modèle** : Ajuster le modèle généré si nécessaire
3. **Snapshot initial** : Créer un snapshot de la base de données existante
4. **Migrations progressives** : Puis ajouter les migrations pour les futurs changements

---

## 12. Outils (Package Manager, CLI)

### Entity Framework Core Tools

Les outils EF Core permettent de gérer les migrations depuis la ligne de commande ou le Package Manager.

### Installation des outils :

#### Outils CLI (EF Core CLI Tool)
Outil en ligne de commande global pour tous les projets
- Installation : Installer globalement sur la machine
- Usage : Utilisable depuis n'importe quel répertoire
- Compatible : Tous les projets .NET utilisant EF Core

#### Package Manager Console
Outil intégré dans Visual Studio
- Disponible dans : Outils > Gestionnaire de paquets NuGet > Console du Gestionnaire de paquets
- Usage : Exécuter les commandes directement depuis VS
- Convenient : Interface graphique en complément de la CLI

### Méthodes d'installation :

#### Méthode 1 : Installation via Package Manager Console
Installer directement via la console dans Visual Studio
- Étapes : Outils > NuGet Package Manager > Package Manager Console
- Package à installer : Microsoft.EntityFrameworkCore.Tools

#### Méthode 2 : Installation via .NET CLI
Installer l'outil globalement sur la machine
- Commande : dotnet tool install --global dotnet-ef
- Permet d'utiliser les commandes EF Core depuis n'importe quel répertoire
- Plus recommandé pour les développements modernes

#### Méthode 3 : Installation comme dépendance de projet
Ajouter l'outil comme dépendance du projet
- Moins recommandé que les deux méthodes précédentes
- Peut être utile pour assurer la compatibilité version

### Package Manager Console (PowerShell)

Interface interactive dans Visual Studio pour gérer les migrations.

**Commandes principales :**

1. **Add-Migration** : Créer une nouvelle migration
   - Détecte les changements du modèle
   - Génère une classe de migration
   - Crée/met à jour le fichier snapshot

2. **Remove-Migration** : Supprimer la dernière migration
   - Uniquement si non appliquée
   - Supprime la classe de migration et met à jour le snapshot

3. **Update-Database** : Appliquer les migrations
   - Applique toutes les migrations pending
   - Peut spécifier une cible particulière
   - Réversible avec un paramètre

4. **Get-Migration** : Lister les migrations
   - Affiche toutes les migrations du projet
   - Indique lesquelles sont appliquées

5. **Script-Migration** : Générer un script SQL
   - Produit le SQL pour appliquer les migrations
   - Utile pour production (révision manuelle)
   - Peut générer les scripts Up et Down

### .NET CLI (Ligne de commande)

Utilisation depuis le terminal / command prompt.

**Commandes principales :**

1. **dotnet ef migrations add** : Créer une migration
   - Similaire à Add-Migration du Package Manager
   - Usage : dotnet ef migrations add NomMigration

2. **dotnet ef migrations remove** : Supprimer une migration
   - Similaire à Remove-Migration du Package Manager
   - Usage : dotnet ef migrations remove

3. **dotnet ef database update** : Appliquer les migrations
   - Similaire à Update-Database du Package Manager
   - Usage : dotnet ef database update ou dotnet ef database update NomMigration

4. **dotnet ef migrations list** : Lister les migrations
   - Similaire à Get-Migration du Package Manager
   - Affiche le statut des migrations (pending/applied)

5. **dotnet ef migrations script** : Générer un script SQL
   - Similaire à Script-Migration du Package Manager
   - Usage : dotnet ef migrations script

6. **dotnet ef database drop** : Supprimer la base de données
   - Attention : Supprime complètement la BD
   - Usage : dotnet ef database drop (avec confirmation)

### Visual Studio GUI

Interface graphique dans Visual Studio pour gérer les migrations.

**Fonctionnalités :**

1. **Package Manager Console** : Exécuter les commandes PowerShell
   - Accès rapide aux commandes
   - Autocomplétion
   - Historique des commandes

2. **Scaffold Existing Database** : Générer le modèle depuis une BD existante
   - Menu : Outils > EF Core Power Tools > Reverse Engineer DbContext
   - Génère automatiquement les entités et le DbContext

3. **View Entity Diagram** : Visualiser le modèle de données
   - Certains add-ins permettent la visualisation
   - Aide à comprendre les relationships

### Comparaison des outils :

| Caractéristique | Package Manager | CLI | GUI |
| --- | --- | --- | --- |
| Interface | PowerShell | Terminal | Visual Studio |
| Utilisation | Intégrée à VS | Indépendante |Visuelle |
| Apprentissage | Moyen | Moyen | Facile |
| Scriptabilité | Facile | Facile | Difficile |
| Flexibilité | Complète | Complète | Limitée |

### Options courantes des commandes :

#### Options Add-Migration / dotnet ef migrations add
- **-OutputDir** : Répertoire de sortie des fichiers de migration
- **-Namespace** : Namespace pour les fichiers de migration
- **-Project** : Projet contenant les migrations
- **-StartupProject** : Projet de démarrage (pour la résolution de dépendances)
- **-Context** : Classe DbContext spécifique à utiliser
- **-Verbose** : Afficher les détails de l'exécution

#### Options Update-Database / dotnet ef database update
- **-Migration** : Cible de la migration (jusqu'à laquelle mettre à jour)
- **-Context** : Classe DbContext spécifique
- **-Project** : Projet cible
- **-Verbose** : Afficher les détails

#### Options Script-Migration / dotnet ef migrations script
- **-From** : Migration de départ pour le script
- **-To** : Migration de fin pour le script
- **-Output** : Fichier de sortie du script SQL
- **-Idempotent** : Rendre le script idempotent

### Intégration CI/CD :

Les outils EF Core s'intègrent avec les pipelines CI/CD :
- **Validation** : Vérifier que les migrations sont correctes
- **Génération de scripts** : Produire les scripts SQL avant déploiement
- **Déploiement** : Appliquer automatiquement les migrations
- **Audit** : Enregistrer les migrations appliquées

### Bonnes pratiques avec les outils :

1. **Utiliser CLI pour le scripting** : Plus facile à automatiser
2. **Utiliser GUI pour le développement** : Plus intuitive
3. **Tester d'abord** : Générer un script SQL avant d'appliquer
4. **Versionner les migrations** : Garder tous les fichiers de migration en Git
5. **Documentation** : Documenter les commandes utilisées pour chaque migration

---

## 13. Debugging Migrations

### Concepts du debugging :

Le debugging des migrations consiste à identifier et résoudre les problèmes lors de la création et l'application des migrations.

### Types d'erreurs à déboguer :

1. **Erreurs de génération** : La migration ne peut pas être générée
2. **Erreurs d'application** : La migration génère une erreur SQL
3. **Erreurs de logique** : La migration s'applique mais produit un résultat incorrect
4. **Erreurs de performance** : La migration est correcte mais trop lente

### Stratégie de debugging :

#### Étape 1 : Activer le logging EF Core
EF Core peut générer des logs détaillés sur son fonctionnement :
- Logs de génération de SQL
- Logs de configuration du modèle
- Logs d'exécution des migrations
- Logs d'erreurs

Le logging peut être activé au niveau du DbContext ou globalement.

#### Étape 2 : Examiner le SQL généré
Le SQL généré par EF Core doit être examiné :
- Vérifier la syntaxe SQL
- Vérifier que les constraints sont correctes
- Vérifier que les types de données sont appropriés
- Utiliser Script-Migration pour voir le SQL complet

#### Étape 3 : Tester sur une copie de la BD
Avant d'appliquer sur la base de données réelle :
- Créer une copie
- Appliquer la migration
- Vérifier le résultat
- Identifier les problèmes

#### Étape 4 : Valider les données
Après l'application :
- Vérifier que les données sont correctes
- Vérifier que les constraints sont appliquées
- Vérifier les indexes
- Tester les requêtes courantes

### Outils de debugging :

#### SQL Server Management Studio (SSMS)
Pour SQL Server :
- Examiner le schéma généré
- Exécuter des requêtes de test
- Vérifier les indexes et constraints
- Tester les performances

#### Autres outils DB
Pour d'autres bases de données :
- PostgreSQL : pgAdmin, DBeaver
- MySQL : MySQL Workbench, DBeaver
- SQLite : SQLite Browser, DBeaver
- DBeaver : Tool universel compatible avec plusieurs BD

#### Visual Studio Debugger
Pour déboguer le code C# :
- Points d'arrêt dans la migration
- Inspection des variables
- Exécution étape par étape
- Inspect des changements trackés

#### EF Core Profiler
Outil commercial pour profiler et déboguer EF Core :
- Voir toutes les requêtes générées
- Performance profiling
- Alertes pour mauvaises patterns

### Checklist de debugging pour les migrations :

1. **Vérifier le modèle**
   - Les entités sont-elles correctement définies ?
   - Les relationships sont-elles correctes ?
   - Les annotations et configurations Fluent API sont-elles appliquées ?

2. **Examiner le SQL généré**
   - Syntaxe correcte ?
   - Types de données appropriés ?
   - Constraints corrects ?

3. **Tester l'application**
   - Migration s'applique sans erreur ?
   - Schéma correspond au modèle ?
   - Données existantes sont-elles préservées ?

4. **Valider les performances**
   - Migration s'exécute rapidement ?
   - Pas de blocages ?
   - Pas d'indexes problématiques ?

5. **Vérifier la compatibilité**
   - Migration compatible avec les versions précédentes ?
   - Pas de breaking changes inattendus ?

### Debugging des erreurs courantes :

#### Erreur : "The entity type 'User' requires a key to be defined"
**Cause :** Pas de propriété marquée comme clé primaire
**Debug :**
- Vérifier que la classe a une propriété Id ou [Key]
- Vérifier la configuration Fluent API HasKey()

#### Erreur : "The model for context 'AppDbContext' has pending changes"
**Cause :** Changements du modèle non migrés
**Debug :**
- Vérifier les modifications du modèle
- Créer une migration
- Appliquer la migration

#### Erreur : "There is no row in the result set"
**Cause :** Migration essaye de mettre à jour des données inexistantes
**Debug :**
- Vérifier les données existantes
- Modifier la migration pour gérer le cas vide
- Utiliser des migrations vides pour seed de données

#### Erreur : "Cannot add a NOT NULL column with no default value"
**Cause :** Ajout d'une colonne non-nullable sans valeur par défaut
**Debug :**
- Rendre la colonne nullable
- Fournir une valeur par défaut
- Créer une migration pour remplir la colonne avant de la rendre non-nullable

### Bonnes pratiques pour éviter les problèmes :

1. **Tester les migrations régulièrement** : Ne pas attendre la production
2. **Créer une sauvegarde** : Avant d'appliquer une migration
3. **Tester sur une copie** : Vérifier le résultat avant d'appliquer sur la vraie BD
4. **Documenter les migrations** : Expliquer le pourquoi et comment
5. **Revoir les migrations** : Code review des migrations avant merge
6. **Monitoring** : Observer les migrations en production
7. **Plan de rollback** : Avoir une stratégie en cas d'erreur

### Logs utiles :

Les logs utiles pour déboguer :

1. **SQL généré** : Pour vérifier la syntaxe et la logique
2. **Warnings** : Pour détecter les problèmes potentiels
3. **Erreurs** : Pour comprendre ce qui s'est mal passé
4. **Performance** : Pour identifier les migrations lentes
5. **Data changes** : Pour tracer les changements de données

### Ressources de debugging :

- Documentation officielle EF Core
- StackOverflow pour les questions courantes
- GitHub issues d'EF Core pour les bugs connus
- Communauté .NET pour les meilleures pratiques
- Logs et monitoring en production

---

## Conclusion

Les migrations Entity Framework Core sont un élément crucial de la gestion du cycle de vie des applications .NET modernes. Une compréhension solide des concepts, des outils et des bonnes pratiques permet de :

- Gérer efficacement l'évolution du schéma
- Collaborer efficacement en équipe
- Déployer en toute confiance en production
- Déboguer et résoudre les problèmes rapidement
- Maintenir une base de données propre et consistante

L'adoption des patterns recommandés et la familiarisation avec les outils disponibles rend le travail avec les migrations plus fluide et réduisent significativement les risques d'erreurs.
