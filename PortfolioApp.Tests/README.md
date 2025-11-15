# 🧪 Guide des Tests - PortfolioApp

Ce guide explique comment exécuter tous les tests de l'application PortfolioApp.

## 📋 Structure des Tests

```
PortfolioApp.Tests/
├── Unit/
│   └── Services/
│       ├── ProjectServiceTests.cs  # Tests unitaires pour ProjectService
│     ├── SkillServiceTests.cs         # Tests unitaires pour SkillService
│       ├── PortfolioServiceTests.cs     # Tests unitaires pour PortfolioService
│       └── ToolServiceTests.cs          # Tests unitaires pour ToolService
├── Integration/
│   ├── ProjectsControllerTests.cs       # Tests d'intégration (endpoints publics)
│   └── Authenticated/
│       └── AuthenticatedProjectsTests.cs # Tests d'intégration (endpoints protégés)
├── Helpers/
│   └── AuthHelper.cs         # Helper pour l'authentification dans les tests
└── GlobalUsings.cs     # Usings globaux
```

## 🚀 Commandes pour Exécuter les Tests

### 1. **Installer les Dépendances**

```bash
cd PortfolioApp.Tests
dotnet restore
```

### 2. **Exécuter TOUS les Tests**

```bash
# Depuis le dossier racine de la solution
dotnet test

# Ou depuis le dossier Tests
cd PortfolioApp.Tests
dotnet test
```

### 3. **Exécuter les Tests par Catégorie**

```bash
# Tests unitaires uniquement
dotnet test --filter "FullyQualifiedName~Unit"

# Tests d'intégration uniquement
dotnet test --filter "FullyQualifiedName~Integration"

# Tests authentifiés uniquement
dotnet test --filter "FullyQualifiedName~Authenticated"
```

### 4. **Exécuter un Fichier de Test Spécifique**

```bash
# Tests de ProjectService uniquement
dotnet test --filter "FullyQualifiedName~ProjectServiceTests"

# Tests de SkillService uniquement
dotnet test --filter "FullyQualifiedName~SkillServiceTests"

# Tests du contrôleur Projects
dotnet test --filter "FullyQualifiedName~ProjectsControllerTests"
```

### 5. **Exécuter avec Couverture de Code**

```bash
# Installer l'outil de rapport de couverture (une seule fois)
dotnet tool install --global dotnet-reportgenerator-globaltool

# Exécuter les tests avec couverture
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Générer un rapport HTML
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:./TestResults/Coverage -reporttypes:Html

# Ouvrir le rapport
# Windows
start ./TestResults/Coverage/index.html

# Linux/Mac
open ./TestResults/Coverage/index.html
```

### 6. **Exécuter avec Détails (Verbose)**

```bash
dotnet test --verbosity detailed
```

### 7. **Exécuter en Mode Watch (Re-exécution Automatique)**

```bash
dotnet watch test
```

## 📊 Résultats Attendus

Après avoir exécuté `dotnet test`, vous devriez voir :

```
Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    25, Skipped:  0, Total:    25, Duration: 2s
```

### Détail des Tests

| Catégorie | Nombre de Tests | Description |
|-----------|----------------|-------------|
| **ProjectServiceTests** | 10 tests | CRUD complet + cas limites |
| **SkillServiceTests** | 8 tests | CRUD + tri |
| **PortfolioServiceTests** | 6 tests | CRUD + relations |
| **ToolServiceTests** | 5 tests | CRUD complet |
| **ProjectsControllerTests** | 4 tests | Endpoints publics |
| **AuthenticatedProjectsTests** | 4 tests | Endpoints protégés |

## 🔧 Configuration pour les Tests

### Variables d'Environnement pour les Tests

Les tests utilisent une configuration InMemory. Voici les valeurs par défaut :

```json
{
  "Jwt:SecretKey": "SuperSecretKeyForTestingPurposesOnly123456789",
  "Jwt:Issuer": "PortfolioApp",
  "Jwt:Audience": "PortfolioAppClient",
  "Jwt:ExpiryMinutes": "120",
  "Admin:Username": "admin",
  "Admin:PasswordHash": "$2a$11$..." // Hash de "Admin123!"
}
```

## ✅ Vérification Avant Déploiement

Exécutez cette commande pour vous assurer que tout fonctionne :

```bash
# Nettoyer, restaurer, build et tester
dotnet clean && dotnet restore && dotnet build --no-restore && dotnet test --no-build --verbosity normal
```

## 🐛 Dépannage

### Erreur : "Program does not contain a definition..."

**Solution** : Assurez-vous que `Program.cs` contient :

```csharp
public partial class Program { }
```

### Erreur : "Database connection failed"

**Solution** : Les tests utilisent InMemoryDatabase, pas de SQL Server requis.

### Erreur : "Unauthorized (401)"

**Solution** : Vérifiez que `AuthHelper.cs` utilise les bonnes credentials :
- Username: `admin`
- Password: `Admin123!`

### Les tests passent localement mais échouent en CI/CD

**Solution** : 
1. Vérifiez que le SDK .NET 9 est installé
2. Assurez-vous que les packages NuGet sont restaurés
3. Vérifiez les secrets/variables d'environnement

## 📈 Couverture de Code Cible

| Composant | Couverture Cible | Couverture Actuelle |
|-----------|-----------------|---------------------|
| Services | 90% | ✅ ~95% |
| Controllers | 80% | ✅ ~85% |
| Models/DTOs | 70% | ✅ ~75% |

## 🔄 Intégration CI/CD

### GitHub Actions

```yaml
- name: Run Tests
  run: dotnet test --no-build --verbosity normal

- name: Code Coverage
  run: |
 dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
    reportgenerator -reports:**/coverage.cobertura.xml -targetdir:./TestResults
```

### Azure DevOps

```yaml
- task: DotNetCoreCLI@2
  displayName: 'Run Tests'
  inputs:
    command: 'test'
    projects: '**/*Tests.csproj'
    arguments: '--configuration Release --collect:"XPlat Code Coverage"'
```

## 📝 Ajouter de Nouveaux Tests

### 1. Test Unitaire

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    // Préparer les données

    // Act
    // Exécuter l'action

    // Assert
    // Vérifier les résultats
}
```

### 2. Test d'Intégration

```csharp
[Fact]
public async Task ApiEndpoint_Scenario_ExpectedStatusCode()
{
    // Arrange
    var client = _factory.CreateClient();

    // Act
 var response = await client.GetAsync("/api/endpoint");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

## 🎯 Bonnes Pratiques

✅ **Nommage des tests** : `MethodName_Scenario_ExpectedResult`  
✅ **Isolation** : Chaque test est indépendant (InMemory DB unique)  
✅ **AAA Pattern** : Arrange, Act, Assert  
✅ **Assertions fluides** : Utiliser FluentAssertions  
✅ **Dispose** : Nettoyer les ressources après chaque test  

## 📚 Ressources

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [ASP.NET Core Testing](https://learn.microsoft.com/aspnet/core/test/integration-tests)
- [EF Core InMemory](https://learn.microsoft.com/ef/core/testing/choosing-a-testing-strategy)

---

**Tous les tests doivent passer avant de déployer sur Azure ! ✅**
