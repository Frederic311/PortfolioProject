# 🚀 GUIDE DE DÉPLOIEMENT AZURE - PORTFOLIO

Ce guide explique comment déployer et mettre à jour votre portfolio sur Azure.

---

## 📋 PRÉREQUIS

- ✅ Azure CLI installé (`az --version`)
- ✅ Docker Desktop installé et démarré
- ✅ .NET 9 SDK installé
- ✅ Compte Azure actif
- ✅ Git installé

---

## 🏗️ ARCHITECTURE AZURE

| Service | Nom | Type | Description |
|---------|-----|------|-------------|
| **Client** | `stclient5g1wo3` | Azure Storage Static Website | Blazor WASM (Frontend) |
| **API** | `portfolio-api-5g1wo3` | Azure App Service (Docker) | ASP.NET Core API (Backend) |
| **Base de données** | `sql-portfolio-5g1wo3` | Azure SQL Database | Base de données SQL Server |
| **Container Registry** | `portfolioacr5g1wo3` | Azure Container Registry | Stockage images Docker |
| **Resource Group** | `rg-portfolio-prod` | Resource Group | Groupe de ressources |

---

## 🌐 URLS DE PRODUCTION

| Service | URL |
|---------|-----|
| **Client (Site)** | https://stclient5g1wo3.z28.web.core.windows.net |
| **API** | https://portfolio-api-5g1wo3.azurewebsites.net |
| **Admin** | https://stclient5g1wo3.z28.web.core.windows.net/admin |

**Login Admin** :
- Username: `admin`
- Password: `admin124`

---

## 🚀 DÉPLOIEMENT COMPLET (PREMIÈRE FOIS)

### **Étape 1 : Créer l'infrastructure avec Terraform**

```powershell
cd terraform

# Initialiser Terraform
terraform init

# Créer le fichier terraform.tfvars avec vos secrets
# (voir section "Configuration des secrets" ci-dessous)

# Appliquer l'infrastructure
terraform apply

# Sauvegarder les outputs
terraform output -json > outputs.json
```

---

### **Étape 2 : Configurer les secrets de l'API**

```powershell
cd PortfolioApp.API

# Générer un secret JWT (32+ caractères)
$jwtSecret = -join ((65..90) + (97..122) + (48..57) | Get-Random -Count 32 | ForEach-Object {[char]$_})

# Configurer les secrets
dotnet user-secrets init
dotnet user-secrets set "Jwt:SecretKey" $jwtSecret
dotnet user-secrets set "Admin:Username" "admin"
dotnet user-secrets set "Admin:PasswordHash" "YOUR_HASHED_PASSWORD"
dotnet user-secrets set "Email:Sender" "your-email@gmail.com"
dotnet user-secrets set "Email:AppPassword" "your-app-password"
```

---

### **Étape 3 : Créer les migrations de base de données**

```powershell
cd PortfolioApp.API

# Créer la migration initiale
dotnet ef migrations add InitialCreate

# Appliquer les migrations localement (test)
dotnet ef database update

# Les migrations seront appliquées automatiquement sur Azure au démarrage de l'API
```

---

## 🔄 MISE À JOUR DU PROJET (APRÈS MODIFICATIONS)

### **🐳 Déployer l'API (Backend)**

```powershell
# 1. Aller à la racine du projet
cd C:\Users\Frederic\Desktop\PortfolioManagment

# 2. Build l'image Docker
docker build -t portfolio-api:latest -f PortfolioApp.API/Dockerfile .

# 3. Tag l'image pour Azure Container Registry
docker tag portfolio-api:latest portfolioacr5g1wo3.azurecr.io/portfolio-api:latest

# 4. Login au Container Registry
az acr login --name portfolioacr5g1wo3

# 5. Push l'image vers Azure
docker push portfolioacr5g1wo3.azurecr.io/portfolio-api:latest

# 6. Redémarrer l'App Service pour charger la nouvelle image
az webapp restart --name portfolio-api-5g1wo3 --resource-group rg-portfolio-prod

# 7. Vérifier les logs
az webapp log tail --name portfolio-api-5g1wo3 --resource-group rg-portfolio-prod
```

**Temps estimé : 3-5 minutes**

---

### **📦 Déployer le Client (Frontend)**

```powershell
# 1. Aller dans le projet Client
cd C:\Users\Frederic\Desktop\PortfolioManagment\PortfolioApp.Client

# 2. Nettoyer les anciens builds
Remove-Item -Recurse -Force bin, obj, publish -ErrorAction SilentlyContinue

# 3. Restore les dépendances
dotnet restore

# 4. Build en mode Release
dotnet publish -c Release -o publish

# 5. Supprimer les anciens fichiers sur Azure
az storage blob delete-batch `
  --account-name stclient5g1wo3 `
  --source '$web' `
  --auth-mode key

# 6. Upload les nouveaux fichiers
az storage blob upload-batch `
  --account-name stclient5g1wo3 `
  --destination '$web' `
  --source publish\wwwroot `
  --overwrite `
  --auth-mode key

# 7. Vérifier le nombre de fichiers uploadés
az storage blob list `
  --account-name stclient5g1wo3 `
  --container-name '$web' `
  --auth-mode key `
  --query "length(@)"
```

**Temps estimé : 1-2 minutes**

---

### **🔄 Déployer TOUT (API + Client)**

Script complet pour déployer les deux en une seule commande :

```powershell
# Script de déploiement complet
$ErrorActionPreference = "Stop"

Write-Host "🚀 DÉPLOIEMENT COMPLET SUR AZURE" -ForegroundColor Green

# ============================================
# 1. API (Backend)
# ============================================
Write-Host "`n🐳 ÉTAPE 1/2 : Déploiement API..." -ForegroundColor Yellow
cd C:\Users\Frederic\Desktop\PortfolioManagment

docker build -t portfolio-api:latest -f PortfolioApp.API/Dockerfile .
docker tag portfolio-api:latest portfolioacr5g1wo3.azurecr.io/portfolio-api:latest
az acr login --name portfolioacr5g1wo3
docker push portfolioacr5g1wo3.azurecr.io/portfolio-api:latest
az webapp restart --name portfolio-api-5g1wo3 --resource-group rg-portfolio-prod

Write-Host "✅ API déployée !" -ForegroundColor Green

# ============================================
# 2. CLIENT (Frontend)
# ============================================
Write-Host "`n📦 ÉTAPE 2/2 : Déploiement Client..." -ForegroundColor Yellow
cd C:\Users\Frederic\Desktop\PortfolioManagment\PortfolioApp.Client

Remove-Item -Recurse -Force bin, obj, publish -ErrorAction SilentlyContinue
dotnet restore
dotnet publish -c Release -o publish

az storage blob delete-batch --account-name stclient5g1wo3 --source '$web' --auth-mode key
az storage blob upload-batch --account-name stclient5g1wo3 --destination '$web' --source publish\wwwroot --overwrite --auth-mode key

Write-Host "✅ Client déployé !" -ForegroundColor Green

# ============================================
# 3. VÉRIFICATION
# ============================================
Write-Host "`n🎉 DÉPLOIEMENT TERMINÉ !" -ForegroundColor Green
Write-Host "Client : https://stclient5g1wo3.z28.web.core.windows.net" -ForegroundColor Cyan
Write-Host "API  : https://portfolio-api-5g1wo3.azurewebsites.net" -ForegroundColor Cyan
Write-Host "Admin  : https://stclient5g1wo3.z28.web.core.windows.net/admin" -ForegroundColor Cyan
```

**Temps estimé : 5-7 minutes**

---

## 🗄️ GESTION DES MIGRATIONS DE BASE DE DONNÉES

### **Créer une nouvelle migration**

```powershell
cd PortfolioApp.API

# Créer la migration
dotnet ef migrations add NomDeLaMigration

# Vérifier la migration créée
dotnet ef migrations list
```

### **Appliquer les migrations sur Azure**

Les migrations sont **appliquées automatiquement** au démarrage de l'API grâce à cette ligne dans `Program.cs` :

```csharp
dbContext.Database.Migrate();
```

**Donc il suffit de redéployer l'API !**

```powershell
# Rebuild et push l'API
docker build -t portfolio-api:latest -f PortfolioApp.API/Dockerfile .
docker tag portfolio-api:latest portfolioacr5g1wo3.azurecr.io/portfolio-api:latest
az acr login --name portfolioacr5g1wo3
docker push portfolioacr5g1wo3.azurecr.io/portfolio-api:latest

# Restart (applique les migrations)
az webapp restart --name portfolio-api-5g1wo3 --resource-group rg-portfolio-prod

# Vérifier dans les logs
az webapp log tail --name portfolio-api-5g1wo3 --resource-group rg-portfolio-prod
```

**Vous devriez voir** :
```
Applying migration '20251115112704_NomDeLaMigration'...
Done.
```

---

## 🐛 DÉPANNAGE

### **Problème : L'API ne démarre pas**

```powershell
# Voir les logs en temps réel
az webapp log tail --name portfolio-api-5g1wo3 --resource-group rg-portfolio-prod

# Télécharger tous les logs
az webapp log download --name portfolio-api-5g1wo3 --resource-group rg-portfolio-prod --log-file logs.zip
```

### **Problème : Le Client affiche une page blanche**

```powershell
# Vérifier que les fichiers sont bien uploadés
az storage blob list `
  --account-name stclient5g1wo3 `
  --container-name '$web' `
  --auth-mode key `
  --query "length(@)"

# Vider le cache du navigateur
# Ctrl + Shift + R (Windows)
# Cmd + Shift + R (Mac)
```

### **Problème : Erreur de connexion SQL**

```powershell
# Vérifier la connexion SQL
az sql db show `
  --resource-group rg-portfolio-prod `
  --server sql-portfolio-5g1wo3 `
  --name portfoliodb

# Vérifier les règles de firewall
az sql server firewall-rule list `
  --resource-group rg-portfolio-prod `
  --server sql-portfolio-5g1wo3
```

### **Problème : 401 Unauthorized**

Vérifier que les secrets JWT sont configurés :

```powershell
# Dans l'App Service Configuration
az webapp config appsettings list `
  --name portfolio-api-5g1wo3 `
  --resource-group rg-portfolio-prod `
  --query "[?name=='Jwt__SecretKey']"
```

---

## 🔒 CONFIGURATION DES SECRETS (terraform.tfvars)

Créer le fichier `terraform/terraform.tfvars` :

```hcl
# terraform/terraform.tfvars
resource_group_name = "rg-portfolio-prod"
location  = "France Central"

sql_admin_login    = "portfolioadmin"
sql_admin_password = "VotreMotDePasseComplexe123!"

jwt_secret_key = "VotreSecretJWT32CaracteresMinimum"

admin_username      = "admin"
admin_password_hash = "AQAAAAIAAYagAAAAEHs..." # Hash BCrypt

email_sender       = "your-email@gmail.com"
email_app_password = "your-app-password"

tags = {
  Environment = "Production"
  Project     = "Portfolio"
  ManagedBy   = "Terraform"
}
```

---

## 📊 COMMANDES UTILES

### **Vérifier l'état des services**

```powershell
# Status de l'App Service
az webapp show --name portfolio-api-5g1wo3 --resource-group rg-portfolio-prod --query "state"

# Status du Storage Account
az storage account show --name stclient5g1wo3 --resource-group rg-portfolio-prod --query "statusOfPrimary"

# Status de la base de données
az sql db show --resource-group rg-portfolio-prod --server sql-portfolio-5g1wo3 --name portfoliodb --query "status"
```

### **Supprimer tous les blobs du Storage**

```powershell
az storage blob delete-batch --account-name stclient5g1wo3 --source '$web' --auth-mode key
```

### **Redémarrer l'App Service**

```powershell
az webapp restart --name portfolio-api-5g1wo3 --resource-group rg-portfolio-prod
```

---

## 📝 CHECKLIST DE DÉPLOIEMENT

- [ ] 1. Code committé et pushé sur GitHub
- [ ] 2. Tests passent localement (`dotnet test`)
- [ ] 3. Secrets configurés (JWT, Admin, Email)
- [ ] 4. Migrations créées si nécessaire
- [ ] 5. Build Docker réussi
- [ ] 6. Push vers Azure Container Registry
- [ ] 7. Restart App Service
- [ ] 8. Client compilé et uploadé
- [ ] 9. Test sur https://stclient5g1wo3.z28.web.core.windows.net
- [ ] 10. Test Admin Login

---

## 🎯 RÉSUMÉ DES COMMANDES RAPIDES

```powershell
# Déployer API uniquement
cd C:\Users\Frederic\Desktop\PortfolioManagment
docker build -t portfolio-api:latest -f PortfolioApp.API/Dockerfile .
docker tag portfolio-api:latest portfolioacr5g1wo3.azurecr.io/portfolio-api:latest
az acr login --name portfolioacr5g1wo3
docker push portfolioacr5g1wo3.azurecr.io/portfolio-api:latest
az webapp restart --name portfolio-api-5g1wo3 --resource-group rg-portfolio-prod

# Déployer Client uniquement
cd C:\Users\Frederic\Desktop\PortfolioManagment\PortfolioApp.Client
Remove-Item -Recurse -Force bin, obj, publish -ErrorAction SilentlyContinue
dotnet publish -c Release -o publish
az storage blob delete-batch --account-name stclient5g1wo3 --source '$web' --auth-mode key
az storage blob upload-batch --account-name stclient5g1wo3 --destination '$web' --source publish\wwwroot --overwrite --auth-mode key
```

---

## 📚 RESSOURCES

- **Azure CLI Documentation** : https://docs.microsoft.com/cli/azure/
- **Docker Documentation** : https://docs.docker.com/
- **Blazor WASM** : https://docs.microsoft.com/aspnet/core/blazor/
- **Terraform Azure Provider** : https://registry.terraform.io/providers/hashicorp/azurerm/

---

## 🆘 SUPPORT

En cas de problème :
1. Vérifier les logs Azure (`az webapp log tail`)
2. Vérifier la console du navigateur (F12)
3. Vérifier que les services Azure sont actifs
4. Vider le cache du navigateur

---

**Dernière mise à jour : 15 novembre 2024**
