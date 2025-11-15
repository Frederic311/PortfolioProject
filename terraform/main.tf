terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {
    key_vault {
      purge_soft_delete_on_destroy    = true
      recover_soft_deleted_key_vaults = true
    }
  }
}

# Random suffix for unique names
resource "random_string" "suffix" {
  length  = 6
  special = false
  upper   = false
}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location
  tags     = var.tags
}

# Azure Container Registry
resource "azurerm_container_registry" "acr" {
  name                = "portfolioacr${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku         = "Basic"
  admin_enabled       = true
  tags    = var.tags
}

# App Service Plan (Linux + Docker)
resource "azurerm_service_plan" "main" {
  name  = "asp-portfolio-${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
os_type       = "Linux"
  sku_name       = "B1"
  tags     = var.tags
}

# App Service for API
resource "azurerm_linux_web_app" "api" {
  name         = "portfolio-api-${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.main.name
  location     = azurerm_resource_group.main.location
  service_plan_id     = azurerm_service_plan.main.id
  https_only       = true
  tags      = var.tags

  site_config {
    always_on = false

    application_stack {
      docker_image_name   = "portfolio-api:latest"
    docker_registry_url = "https://${azurerm_container_registry.acr.login_server}"
    }

 cors {
      allowed_origins = [
        "https://${azurerm_storage_account.client.primary_web_host}",
        "http://localhost:8080"
    ]
      support_credentials = true
    }
  }

  app_settings = {
    "WEBSITES_PORT"          = "8080"
    "ASPNETCORE_ENVIRONMENT"      = "Production"
    "DOCKER_REGISTRY_SERVER_URL"       = "https://${azurerm_container_registry.acr.login_server}"
    "DOCKER_REGISTRY_SERVER_USERNAME"      = azurerm_container_registry.acr.admin_username
    "DOCKER_REGISTRY_SERVER_PASSWORD"      = azurerm_container_registry.acr.admin_password
    "ConnectionStrings__DefaultConnection" = "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.main.name};User ID=${var.sql_admin_login};Password=${var.sql_admin_password};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    "Jwt__SecretKey"         = var.jwt_secret_key
    "Jwt__Issuer"          = "PortfolioApp"
    "Jwt__Audience"      = "PortfolioAppClient"
    "Jwt__ExpiryMinutes"              = "120"
    "Admin__Username"         = var.admin_username
 "Admin__PasswordHash"    = var.admin_password_hash
    "Email__SenderEmail"     = var.email_sender
    "Email__AppPassword"     = var.email_app_password
  }

  identity {
    type = "SystemAssigned"
  }

  depends_on = [
    azurerm_key_vault.main,
    azurerm_mssql_database.main
  ]
}

# Azure SQL Server
resource "azurerm_mssql_server" "main" {
  name         = "sql-portfolio-${random_string.suffix.result}"
  resource_group_name    = azurerm_resource_group.main.name
  location      = azurerm_resource_group.main.location
  version            = "12.0"
  administrator_login   = var.sql_admin_login
  administrator_login_password = var.sql_admin_password
  tags      = var.tags
}

# Azure SQL Database
resource "azurerm_mssql_database" "main" {
  name      = "portfoliodb"
  server_id = azurerm_mssql_server.main.id
  sku_name  = "Basic"
  tags      = var.tags
}

# SQL Firewall Rules
resource "azurerm_mssql_firewall_rule" "allow_azure" {
  name    = "AllowAzureServices"
  server_id    = azurerm_mssql_server.main.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

# Key Vault
data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "main" {
  name  = "kv-portfolio-${random_string.suffix.result}"
  location      = azurerm_resource_group.main.location
  resource_group_name  = azurerm_resource_group.main.name
  tenant_id       = data.azurerm_client_config.current.tenant_id
  sku_name       = "standard"
  soft_delete_retention_days = 7
  purge_protection_enabled   = false
  tags       = var.tags

  # Access policy for current user (Terraform)
  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    secret_permissions = [
      "Get", "List", "Set", "Delete", "Purge", "Recover"
    ]
  }
}

# Separate access policy for App Service (avoid cycle)
resource "azurerm_key_vault_access_policy" "api" {
  key_vault_id = azurerm_key_vault.main.id
  tenant_id    = azurerm_linux_web_app.api.identity[0].tenant_id
  object_id    = azurerm_linux_web_app.api.identity[0].principal_id

  secret_permissions = [
    "Get", "List"
  ]

  depends_on = [
    azurerm_linux_web_app.api
  ]
}

# Secrets in Key Vault
resource "azurerm_key_vault_secret" "sql_connection" {
  name         = "SqlConnectionString"
  value        = "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.main.name};User ID=${var.sql_admin_login};Password=${var.sql_admin_password};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  key_vault_id = azurerm_key_vault.main.id
}

resource "azurerm_key_vault_secret" "jwt_secret" {
  name    = "JwtSecretKey"
value        = var.jwt_secret_key
  key_vault_id = azurerm_key_vault.main.id
}

resource "azurerm_key_vault_secret" "admin_username" {
  name         = "AdminUsername"
  value        = var.admin_username
  key_vault_id = azurerm_key_vault.main.id
}

resource "azurerm_key_vault_secret" "admin_password_hash" {
  name         = "AdminPasswordHash"
  value        = var.admin_password_hash
  key_vault_id = azurerm_key_vault.main.id
}

resource "azurerm_key_vault_secret" "email_sender" {
  name         = "EmailSenderEmail"
value        = var.email_sender
  key_vault_id = azurerm_key_vault.main.id
}

resource "azurerm_key_vault_secret" "email_app_password" {
  name  = "EmailAppPassword"
  value    = var.email_app_password
  key_vault_id = azurerm_key_vault.main.id
}

# Storage Account for Blazor WASM Client (Static Website)
resource "azurerm_storage_account" "client" {
  name         = "stclient${random_string.suffix.result}"
  resource_group_name    = azurerm_resource_group.main.name
  location       = azurerm_resource_group.main.location
  account_tier   = "Standard"
  account_replication_type = "LRS"
  tags          = var.tags

  static_website {
  index_document = "index.html"
  }
}

# Generate Ansible inventory file
resource "local_file" "ansible_inventory" {
  content = templatefile("${path.module}/templates/inventory.tpl", {
 resource_group_name = azurerm_resource_group.main.name
    api_app_name        = azurerm_linux_web_app.api.name
    acr_name            = azurerm_container_registry.acr.name
    acr_login_server    = azurerm_container_registry.acr.login_server
    acr_username        = azurerm_container_registry.acr.admin_username
    acr_password        = azurerm_container_registry.acr.admin_password
storage_account     = azurerm_storage_account.client.name
    sql_server_fqdn     = azurerm_mssql_server.main.fully_qualified_domain_name
    sql_database        = azurerm_mssql_database.main.name
    sql_admin_login = var.sql_admin_login
    sql_admin_password  = var.sql_admin_password
  })
  filename = "${path.module}/../ansible/inventory/azure_resources.yml"
}
