# Portfolio Management API

## ✅ Architecture complète créée

### Structure du projet

```
PortfolioApp.API/
├── Controllers/           # Endpoints HTTP
│   ├── PortfoliosController.cs
│   ├── ProjectsController.cs
│   ├── SkillsController.cs
│   └── ToolsController.cs
├── Services/             # Logique métier
│   ├── IPortfolioService.cs
│   ├── PortfolioService.cs
│   ├── IProjectService.cs
│   ├── ProjectService.cs
│   ├── ISkillService.cs
│   ├── SkillService.cs
│   ├── IToolService.cs
│   └── ToolService.cs
├── DTOs/                 # Data Transfer Objects
│   ├── PortfolioDto.cs
│   ├── ProjectDto.cs
│   ├── SkillDto.cs
│   └── ToolDto.cs
├── Data/                 # DbContext
│   └── AppDbContext.cs
├── Migrations/           # Migrations EF Core
├── Program.cs            # Configuration de l'app
└── test-api.http        # Tests HTTP
```

## 🚀 Démarrage

### 1. Lancer l'API
```bash
cd PortfolioApp.API
dotnet run
```

L'API sera disponible sur : `http://localhost:5158` et `https://localhost:7026`

### 2. Tester l'API

Ouvrez le fichier `test-api.http` et utilisez l'extension REST Client de VS Code pour tester les endpoints.

## 📡 Endpoints disponibles

### Portfolios
- `GET /api/portfolios` - Liste tous les portfolios
- `GET /api/portfolios/{id}` - Récupère un portfolio par ID
- `POST /api/portfolios` - Crée un nouveau portfolio
- `PUT /api/portfolios/{id}` - Met à jour un portfolio
- `DELETE /api/portfolios/{id}` - Supprime un portfolio

### Projects
- `GET /api/projects` - Liste tous les projets
- `GET /api/projects/portfolio/{portfolioId}` - Projets d'un portfolio
- `GET /api/projects/{id}` - Récupère un projet par ID
- `POST /api/projects` - Crée un nouveau projet
- `PUT /api/projects/{id}` - Met à jour un projet
- `DELETE /api/projects/{id}` - Supprime un projet
- `POST /api/projects/{id}/images` - Ajoute une image à un projet
- `DELETE /api/projects/{id}/images/{imageId}` - Supprime une image

### Skills
- `GET /api/skills` - Liste toutes les compétences
- `GET /api/skills/portfolio/{portfolioId}` - Skills d'un portfolio
- `GET /api/skills/{id}` - Récupère une compétence par ID
- `POST /api/skills` - Crée une nouvelle compétence
- `PUT /api/skills/{id}` - Met à jour une compétence
- `DELETE /api/skills/{id}` - Supprime une compétence

### Tools
- `GET /api/tools` - Liste tous les outils
- `GET /api/tools/skill/{skillId}` - Tools d'une skill
- `GET /api/tools/{id}` - Récupère un outil par ID
- `POST /api/tools` - Crée un nouvel outil
- `PUT /api/tools/{id}` - Met à jour un outil
- `DELETE /api/tools/{id}` - Supprime un outil

## 🔧 Configuration

### Base de données
La chaîne de connexion est dans `appsettings.json` :
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=PortfolioDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

### CORS
Le CORS est configuré pour autoriser les requêtes depuis :
- `https://localhost:7167`
- `http://localhost:5254`

## 📝 Workflow de test complet

1. **Créer un portfolio**
```http
POST https://localhost:7026/api/portfolios
Content-Type: application/json

{
  "name": "Mon Portfolio",
  "slogan": "Full-Stack Developer",
  "description": "Portfolio de démonstration"
}
```

2. **Créer une skill (Languages)**
```http
POST https://localhost:7026/api/skills
Content-Type: application/json

{
  "name": "Languages",
  "portfolioId": 1
}
```

3. **Créer des tools (C#, TypeScript, Python)**
```http
POST https://localhost:7026/api/tools
Content-Type: application/json

{
  "name": "C#",
  "skillId": 1
}
```

4. **Créer un projet**
```http
POST https://localhost:7026/api/projects
Content-Type: application/json

{
  "name": "Portfolio App",
  "description": "Application Blazor",
  "status": "EnCours",
  "startDate": "2025-11-01T00:00:00",
  "portfolioId": 1,
  "toolIds": [1, 2]
}
```

5. **Voir tous les skills du portfolio**
```http
GET https://localhost:7026/api/skills/portfolio/1
```

## 🎯 Prochaines étapes

1. ✅ **API Backend** - TERMINÉ
2. 🔄 **Frontend Blazor** - Créer les pages pour afficher et gérer les données
3. 🔄 **Authentification** - Ajouter l'authentification pour protéger l'admin
4. 🔄 **Déploiement Azure** - Terraform + CI/CD

## 💡 Notes techniques

- **Auto-migration** : La BD est automatiquement créée/mise à jour au démarrage
- **DTOs** : Séparation entre les entités de la BD et les objets exposés par l'API
- **Services** : Couche de logique métier séparée des contrôleurs
- **Enum Status** : Les statuts de projet sont stockés comme strings dans la BD
- **Images Base64** : Les images sont stockées en Base64 (à remplacer par Azure Blob Storage en prod)
