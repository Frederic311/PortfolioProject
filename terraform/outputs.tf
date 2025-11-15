output "resource_group_name" {
  description = "Resource group name"
  value   = azurerm_resource_group.main.name
}

output "api_url" {
  description = "API URL"
  value       = "https://${azurerm_linux_web_app.api.default_hostname}"
}

output "client_url" {
description = "Blazor WASM client URL"
  value       = azurerm_storage_account.client.primary_web_endpoint
}

output "acr_login_server" {
  description = "ACR Login Server"
  value       = azurerm_container_registry.acr.login_server
}

output "acr_name" {
  description = "ACR name"
  value       = azurerm_container_registry.acr.name
}

output "sql_server_fqdn" {
  description = "SQL Server FQDN"
  value       = azurerm_mssql_server.main.fully_qualified_domain_name
}

output "sql_database_name" {
  description = "Database name"
  value= azurerm_mssql_database.main.name
}

output "key_vault_name" {
  description = "Key Vault name"
  value       = azurerm_key_vault.main.name
}

output "storage_account_name" {
  description = "Storage Account name"
  value   = azurerm_storage_account.client.name
}
