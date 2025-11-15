# Frontend Architecture - PortfolioApp.Client

## 🏗️ Architecture mise en place

### Structure des dossiers

```
PortfolioApp.Client/
├── Components/
│   ├── Auth/
│   │   └── AuthorizeView.razor       # Composant de protection des routes
│   ├── Pages/
│   │   ├── Home.razor       # Page d'accueil protégée
│   │   └── Login.razor   # Page de connexion
│   ├── Layout/
│   │   └── MainLayout.razor
│   └── _Imports.razor
├── Services/
│   ├── IAuthService.cs    # Interface du service d'authentification
│   └── AuthService.cs       # Implémentation du service
├── Program.cs   # Configuration DI et middleware
└── appsettings.json   # Configuration (URL API)
```

---

## 🎯 Fonctionnalités implémentées

### 1. Service d'authentification (`AuthService`)

**Responsabilités** :
- Communication avec l'API backend (`/api/auth/login`)
- Gestion du token JWT dans `localStorage`
- Vérification de l'expiration du token
- Configuration automatique du header `Authorization`

**Méthodes** :
- `LoginAsync(username, password)` - Authentification
- `LogoutAsync()` - Déconnexion et nettoyage
- `IsAuthenticatedAsync()` - Vérification de l'état d'authentification
- `GetTokenAsync()` - Récupération du token stocké

### 2. Composant de protection (`AuthorizeView`)

- Vérifie l'authentification avant d'afficher le contenu
- Redirige vers `/login` si non authentifié
- Affiche un indicateur de chargement pendant la vérification

### 3. Page de Login (`/login`)

**Design Tailwind CSS** :
- Fond dégradé moderne (indigo → purple → pink)
- Formulaire glassmorphism (backdrop-blur)
- Validation avec `DataAnnotationsValidator`
- Messages d'erreur stylisés
- Animation de chargement pendant la connexion
- Responsive (mobile-first)

**Fonctionnalités** :
- Validation côté client
- Gestion des erreurs
- Redirection automatique vers `/` après connexion
- Affichage des credentials de démo

### 4. Page Home protégée (`/`)

**Protections** :
- Enveloppée dans `<AuthorizeView>`
- Accessible uniquement si authentifié
- Affiche un bouton de déconnexion

**Features** :
- Badge de statut (authentifié)
- Liste des prochaines étapes
- Boutons pour les futures fonctionnalités

---

## 🚀 Utilisation

### Lancer l'application

```bash
cd PortfolioApp.Client
dotnet run
```

L'application sera disponible sur : `https://localhost:7167`

### Flux d'authentification

1. **Accès à la page Home** (`/`)
   - Si non authentifié → redirection vers `/login`

2. **Page de connexion** (`/login`)
   - Entrez les credentials : `admin` / `admin124`
   - Cliquez sur "Se connecter"
   - Le token JWT est stocké dans `localStorage`

3. **Redirection vers Home**
   - Affichage de la page protégée
   - Token envoyé automatiquement dans les futures requêtes API

4. **Déconnexion**
   - Cliquez sur "Déconnexion"
   - Token supprimé de `localStorage`
   - Redirection vers `/login`

---

## 🔧 Configuration

### Changer l'URL de l'API

Modifiez `appsettings.json` :

```json
{
  "ApiSettings": {
    "BaseUrl": "https://votre-api.com"
  }
}
```

Ou utilisez les variables d'environnement en production :

```bash
export ApiSettings__BaseUrl="https://production-api.com"
```

---

## 🎨 Design System (Tailwind CSS)

### Couleurs principales

- **Gradient background** : `from-indigo-600 via-purple-600 to-pink-500`
- **Glassmorphism** : `bg-white/10 backdrop-blur-md`
- **Bordures** : `ring-1 ring-white/20`
- **Hover effects** : `hover:-translate-y-0.5`

### Composants réutilisables

**Bouton principal** :
```html
<button class="transform rounded-md bg-white px-5 py-2 font-semibold text-indigo-700 transition hover:-translate-y-0.5 hover:bg-white/90">
  Texte du bouton
</button>
```

**Input field** :
```html
<input class="w-full rounded-lg bg-white/10 px-4 py-3 text-white placeholder-white/50 ring-1 ring-white/20 transition focus:bg-white/20 focus:outline-none focus:ring-2 focus:ring-white/40" />
```

**Badge de statut** :
```html
<span class="inline-flex items-center rounded-full bg-green-500/20 px-3 py-1 text-sm font-medium ring-1 ring-green-500/30">
  <span class="mr-2 h-2 w-2 rounded-full bg-green-400"></span>
  Texte
</span>
```

---

## 🔐 Sécurité

### Bonnes pratiques appliquées

1. **Token JWT stocké dans localStorage**
   - Alternative : `sessionStorage` (plus sécurisé, expire à la fermeture du navigateur)
   
2. **Vérification de l'expiration**
   - Le service vérifie automatiquement si le token est expiré
   - Nettoyage automatique des tokens expirés

3. **Header Authorization automatique**
   - Le token est ajouté à toutes les requêtes HTTP vers l'API

4. **Protection des routes**
   - `AuthorizeView` bloque l'accès aux pages protégées

### ⚠️ Améliorations recommandées pour la production

1. **Utiliser httpOnly cookies** au lieu de `localStorage` (plus sécurisé contre XSS)
2. **Implémenter le refresh token** pour renouveler automatiquement les sessions
3. **Ajouter CSRF protection** pour les actions sensibles
4. **Logger les tentatives d'accès non autorisées**

---

## 📋 Prochaines étapes

### Pages à créer

- [ ] `/portfolios` - Liste et gestion des portfolios
- [ ] `/portfolios/{id}` - Détail d'un portfolio
- [ ] `/projects` - Gestion des projets
- [ ] `/skills` - Gestion des compétences
- [ ] `/profile` - Profil utilisateur

### Composants à créer

- [ ] `Navbar.razor` - Barre de navigation
- [ ] `LoadingSpinner.razor` - Composant de chargement réutilisable
- [ ] `ErrorBoundary.razor` - Gestion des erreurs
- [ ] `Toast.razor` - Notifications

### Services à créer

- [ ] `PortfolioService.cs` - Gestion des portfolios
- [ ] `ProjectService.cs` - Gestion des projets
- [ ] `SkillService.cs` - Gestion des compétences
- [ ] `ToastService.cs` - Notifications

---

## 🧪 Tests

### Tester le flux complet

1. **Lancer l'API** :
   ```bash
   cd PortfolioApp.API
 dotnet run
   ```

2. **Lancer le Client** :
   ```bash
   cd PortfolioApp.Client
   dotnet run
   ```

3. **Tester** :
   - Accédez à `https://localhost:7167`
   - Vous devriez être redirigé vers `/login`
   - Connectez-vous avec `admin` / `admin124`
 - Vérifiez que vous êtes redirigé vers `/`
   - Testez la déconnexion

### Vérifier le token dans le navigateur

Ouvrez la console du navigateur (F12) :

```javascript
// Voir le token stocké
localStorage.getItem('authToken')

// Voir l'expiration
localStorage.getItem('authExpiry')

// Supprimer manuellement
localStorage.removeItem('authToken')
localStorage.removeItem('authExpiry')
```

---

## 📚 Ressources

- [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [Tailwind CSS](https://tailwindcss.com/docs)
- [JWT Best Practices](https://cheatsheetseries.owasp.org/cheatsheets/JSON_Web_Token_for_Java_Cheat_Sheet.html)
- [Blazor Authentication](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/)

---

## 💡 Notes techniques

### Pourquoi Blazor Server au lieu de WebAssembly ?

- **Blazor Server** : Moins de latence pour les actions, meilleur pour les apps admin
- Connexion SignalR maintenue avec le serveur
- Pas de téléchargement initial de l'assemblée .NET

### localStorage vs sessionStorage

- **localStorage** : Persiste après fermeture du navigateur (choix actuel)
- **sessionStorage** : Plus sécurisé, expire à la fermeture
- **httpOnly cookies** : Le plus sécurisé (recommandé en prod)

### Gestion du state global

Pour un state management plus avancé, considérez :
- **Fluxor** (Redux pour Blazor)
- **Blazor State Container** (natif)
- **MediatR** (CQRS pattern)
