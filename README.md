# PortfolioProject

Projet full-stack pour gérer et exposer un portfolio (projets, compétences, outils, CV, contact), avec une API ASP.NET Core, un client Blazor WebAssembly, des tests automatisés et une infrastructure Azure via Terraform.

## Vue d'ensemble

Ce repository contient :

- **Backend API** (`/PortfolioApp.API`) : API REST ASP.NET Core (.NET 9), EF Core SQL Server, JWT, CORS.
- **Frontend Client** (`/PortfolioApp.Client`) : application Blazor WebAssembly (.NET 9).
- **Bibliothèque partagée** (`/PortfolioApp.Shared`) : modèles/DTOs/enums partagés.
- **Tests** (`/PortfolioApp.Tests`) : tests unitaires et d’intégration (xUnit, FluentAssertions, Moq).
- **Infra as Code** (`/terraform`) : provisioning Azure (App Service, SQL, ACR, Storage, Key Vault).
- **Conteneurisation** : Dockerfiles API/Client + `docker-compose.yml`.

## Architecture du projet

```text
PortfolioProject/
├── PortfolioApp.API/
├── PortfolioApp.Client/
├── PortfolioApp.Shared/
├── PortfolioApp.Tests/
├── terraform/
├── docker-compose.yml
├── run-tests.sh
└── run-tests.ps1
```

## Fonctionnalités principales

- Gestion des **portfolios**, **projects**, **skills**, **tools**
- Authentification **JWT** pour les routes protégées
- Endpoints **CV** et **contact**
- Front-office Blazor + espace admin protégé
- Migrations EF Core appliquées au démarrage de l’API (hors environnement de test)

## Prérequis

- **.NET SDK 9.0**
- (Optionnel) **Docker** / Docker Compose
- (Optionnel) **Terraform** + **Azure CLI** pour le déploiement cloud

## Démarrage local rapide

### 1) Restaurer les dépendances

```bash
dotnet restore
```

### 2) Lancer l’API

```bash
cd /home/runner/work/PortfolioProject/PortfolioProject/PortfolioApp.API
dotnet run
```

### 3) Lancer le client

```bash
cd /home/runner/work/PortfolioProject/PortfolioProject/PortfolioApp.Client
dotnet run
```

## Build et tests

### Build solution

```bash
cd /home/runner/work/PortfolioProject/PortfolioProject
dotnet build
```

### Exécuter les tests

```bash
cd /home/runner/work/PortfolioProject/PortfolioProject
dotnet test /home/runner/work/PortfolioProject/PortfolioProject/PortfolioApp.Tests/PortfolioApp.Tests.csproj
```

Scripts utilitaires disponibles :

- `./run-tests.sh` (Linux/macOS)
- `./run-tests.ps1` (Windows/PowerShell)

## Lancement avec Docker Compose

```bash
cd /home/runner/work/PortfolioProject/PortfolioProject
docker compose up --build
```

Services principaux :

- API : `http://localhost:5000`
- Client : `http://localhost:8080`
- SQL Server : `localhost:1433`

## Déploiement Azure

L’infrastructure et le process de déploiement sont documentés dans :

- `/home/runner/work/PortfolioProject/PortfolioProject/terraform`
- `/home/runner/work/PortfolioProject/PortfolioProject/AZURE_DEPLOYMENT_GUIDE.md`

## Documentation complémentaire

- API : `/home/runner/work/PortfolioProject/PortfolioProject/PortfolioApp.API/README.md`
- Client : `/home/runner/work/PortfolioProject/PortfolioProject/PortfolioApp.Client/README.md`
- Tests : `/home/runner/work/PortfolioProject/PortfolioProject/PortfolioApp.Tests/README.md`

---
