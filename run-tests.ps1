#!/usr/bin/env pwsh
# Script pour exécuter les tests de PortfolioApp

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("all", "unit", "integration", "coverage", "watch")]
  [string]$TestType = "all",
    
    [Parameter(Mandatory=$false)]
    [switch]$Verbose
)

$ErrorActionPreference = "Stop"

Write-Host "🧪 PortfolioApp Test Runner" -ForegroundColor Cyan
Write-Host "=============================" -ForegroundColor Cyan
Write-Host ""

# Naviguer vers le dossier de tests
$testPath = Join-Path $PSScriptRoot "PortfolioApp.Tests"

if (-not (Test-Path $testPath)) {
    Write-Host "❌ Le dossier de tests n'existe pas : $testPath" -ForegroundColor Red
    exit 1
}

Set-Location $testPath

# Fonction pour exécuter les tests
function Run-Tests {
    param(
    [string]$Filter = "",
        [string]$Description = ""
    )
    
    Write-Host "▶️  $Description" -ForegroundColor Yellow
    Write-Host ""
    
    $verbosity = if ($Verbose) { "detailed" } else { "normal" }
    
    if ($Filter) {
  dotnet test --verbosity $verbosity --filter $Filter
    } else {
        dotnet test --verbosity $verbosity
    }
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "❌ Tests échoués!" -ForegroundColor Red
        exit 1
    }
    
Write-Host ""
    Write-Host "✅ Tests réussis!" -ForegroundColor Green
    Write-Host ""
}

# Fonction pour la couverture de code
function Run-Coverage {
    Write-Host "📊 Génération du rapport de couverture de code..." -ForegroundColor Yellow
    Write-Host ""
    
    # Exécuter les tests avec couverture
    dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/
    
    if ($LASTEXITCODE -ne 0) {
Write-Host ""
   Write-Host "❌ Génération de la couverture échouée!" -ForegroundColor Red
        exit 1
    }
    
    # Vérifier si reportgenerator est installé
 $reportGeneratorInstalled = (dotnet tool list --global | Select-String "dotnet-reportgenerator-globaltool")
    
    if (-not $reportGeneratorInstalled) {
        Write-Host "📦 Installation de reportgenerator..." -ForegroundColor Yellow
        dotnet tool install --global dotnet-reportgenerator-globaltool
    }
    
    # Générer le rapport HTML
    Write-Host "📈 Génération du rapport HTML..." -ForegroundColor Yellow
    reportgenerator -reports:**/coverage.cobertura.xml -targetdir:./TestResults/Coverage -reporttypes:Html
    
    if ($LASTEXITCODE -ne 0) {
      Write-Host ""
        Write-Host "❌ Génération du rapport échouée!" -ForegroundColor Red
        exit 1
    }
    
    Write-Host ""
    Write-Host "✅ Rapport de couverture généré!" -ForegroundColor Green
    Write-Host "📂 Ouvrir : TestResults/Coverage/index.html" -ForegroundColor Cyan
    Write-Host ""
    
    # Ouvrir le rapport
    $reportPath = Join-Path $testPath "TestResults" "Coverage" "index.html"
    if (Test-Path $reportPath) {
  Write-Host "🌐 Ouverture du rapport dans le navigateur..." -ForegroundColor Yellow
        Start-Process $reportPath
    }
}

# Exécuter selon le type choisi
switch ($TestType) {
    "all" {
        Run-Tests -Description "Exécution de TOUS les tests"
    }
    
 "unit" {
        Run-Tests -Filter "FullyQualifiedName~Unit" -Description "Exécution des tests UNITAIRES"
    }
    
    "integration" {
        Run-Tests -Filter "FullyQualifiedName~Integration" -Description "Exécution des tests D'INTÉGRATION"
  }
    
    "coverage" {
     Run-Coverage
    }
    
    "watch" {
Write-Host "👀 Mode Watch activé - Les tests se réexécuteront automatiquement" -ForegroundColor Yellow
    Write-Host "   Appuyez sur Ctrl+C pour arrêter" -ForegroundColor Gray
        Write-Host ""
        dotnet watch test
    }
}

Write-Host ""
Write-Host "✨ Terminé!" -ForegroundColor Green
