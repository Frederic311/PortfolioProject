#!/bin/bash

# Script pour exécuter les tests de PortfolioApp

set -e

# Couleurs
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${CYAN}🧪 PortfolioApp Test Runner${NC}"
echo -e "${CYAN}=============================${NC}"
echo ""

# Fonction d'aide
show_help() {
    echo "Usage: ./run-tests.sh [option]"
    echo ""
  echo "Options:"
    echo "  all           Exécuter tous les tests (par défaut)"
    echo "  unit          Exécuter seulement les tests unitaires"
    echo "  integration   Exécuter seulement les tests d'intégration"
    echo "  coverage      Générer un rapport de couverture de code"
    echo "  watch Mode watch (réexécution automatique)"
    echo "  help          Afficher cette aide"
    echo ""
}

# Fonction pour exécuter les tests
run_tests() {
    local filter=$1
 local description=$2
    
    echo -e "${YELLOW}▶️  $description${NC}"
    echo ""
    
    if [ -n "$filter" ]; then
        dotnet test --verbosity normal --filter "$filter"
    else
        dotnet test --verbosity normal
    fi
    
    if [ $? -ne 0 ]; then
     echo ""
        echo -e "${RED}❌ Tests échoués!${NC}"
   exit 1
    fi
    
    echo ""
    echo -e "${GREEN}✅ Tests réussis!${NC}"
    echo ""
}

# Fonction pour la couverture de code
run_coverage() {
    echo -e "${YELLOW}📊 Génération du rapport de couverture de code...${NC}"
    echo ""
    
    # Exécuter les tests avec couverture
  dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/
    
    if [ $? -ne 0 ]; then
     echo ""
        echo -e "${RED}❌ Génération de la couverture échouée!${NC}"
        exit 1
    fi
    
    # Vérifier si reportgenerator est installé
    if ! dotnet tool list --global | grep -q "dotnet-reportgenerator-globaltool"; then
        echo -e "${YELLOW}📦 Installation de reportgenerator...${NC}"
        dotnet tool install --global dotnet-reportgenerator-globaltool
    fi
    
    # Générer le rapport HTML
    echo -e "${YELLOW}📈 Génération du rapport HTML...${NC}"
    reportgenerator -reports:**/coverage.cobertura.xml -targetdir:./TestResults/Coverage -reporttypes:Html
    
if [ $? -ne 0 ]; then
        echo ""
    echo -e "${RED}❌ Génération du rapport échouée!${NC}"
        exit 1
    fi
    
    echo ""
    echo -e "${GREEN}✅ Rapport de couverture généré!${NC}"
    echo -e "${CYAN}📂 Ouvrir : TestResults/Coverage/index.html${NC}"
    echo ""
 
    # Ouvrir le rapport (si possible)
  if [ -f "./TestResults/Coverage/index.html" ]; then
     echo -e "${YELLOW}🌐 Ouverture du rapport dans le navigateur...${NC}"
        if command -v xdg-open > /dev/null; then
 xdg-open "./TestResults/Coverage/index.html"
        elif command -v open > /dev/null; then
            open "./TestResults/Coverage/index.html"
        fi
    fi
}

# Naviguer vers le dossier de tests
cd "$(dirname "$0")/PortfolioApp.Tests" || exit 1

# Traiter les arguments
case "${1:-all}" in
    all)
  run_tests "" "Exécution de TOUS les tests"
     ;;
    unit)
        run_tests "FullyQualifiedName~Unit" "Exécution des tests UNITAIRES"
    ;;
  integration)
        run_tests "FullyQualifiedName~Integration" "Exécution des tests D'INTÉGRATION"
        ;;
    coverage)
        run_coverage
        ;;
    watch)
        echo -e "${YELLOW}👀 Mode Watch activé - Les tests se réexécuteront automatiquement${NC}"
   echo -e "${NC}   Appuyez sur Ctrl+C pour arrêter${NC}"
echo ""
        dotnet watch test
        ;;
    help)
     show_help
        exit 0
        ;;
    *)
        echo -e "${RED}❌ Option invalide: $1${NC}"
   echo ""
        show_help
        exit 1
        ;;
esac

echo ""
echo -e "${GREEN}✨ Terminé!${NC}"
