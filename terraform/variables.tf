variable "resource_group_name" {
  description = "Azure resource group name"
  type        = string
  default     = "rg-portfolio-prod"
}

variable "location" {
  description = "Azure region"
  type        = string
  default     = "France Central"
}

variable "sql_admin_login" {
  description = "SQL Server admin username"
  type   = string
  sensitive   = true
}

variable "sql_admin_password" {
  description = "SQL Server admin password"
  type   = string
  sensitive   = true
}

variable "jwt_secret_key" {
  description = "JWT Secret Key"
  type     = string
  sensitive   = true
}

variable "admin_username" {
  description = "Application admin username"
  type        = string
  sensitive   = true
  default     = "admin"
}

variable "admin_password_hash" {
  description = "Application admin password hash"
type     = string
  sensitive   = true
}

variable "email_sender" {
  description = "Email sender address"
  type        = string
  sensitive   = true
  default     = "onana.frederic@institutsaintjean.org"
}

variable "email_app_password" {
  description = "Email app password"
  type      = string
  sensitive   = true
  default     = "fbss usbg agas guxh"
}

variable "tags" {
  description = "Tags for Azure resources"
  type        = map(string)
  default = {
    Environment = "Production"
    Project     = "Portfolio"
    ManagedBy   = "Terraform"
  }
}
