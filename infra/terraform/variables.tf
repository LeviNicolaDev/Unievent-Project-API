variable "subscription_id" {
  description = "Azure subscription ID (az account show --query id -o tsv)."
  type        = string
}

variable "location" {
  description = "Azure region for the API and PostgreSQL. Change it if unavailable in the student subscription."
  type        = string
  default     = "canadacentral"
}

variable "web_url" {
  description = "Production HTTPS origin of the frontend hosted on Vercel, without a path or trailing slash."
  type        = string

  validation {
    condition     = can(regex("^https://[A-Za-z0-9]([A-Za-z0-9.-]*[A-Za-z0-9])?$", var.web_url))
    error_message = "web_url must be an HTTPS origin, for example https://unievent.vercel.app, without a path or trailing slash."
  }
}

variable "project_name" {
  description = "Lowercase base name used for Azure resources."
  type        = string
  default     = "unievent"

  validation {
    condition     = can(regex("^[a-z0-9-]{3,20}$", var.project_name))
    error_message = "project_name must contain 3-20 lowercase letters, numbers, or hyphens."
  }
}

variable "container_image" {
  description = "Public GHCR image containing the API, for example ghcr.io/levinicoladev/unievent-project-api:latest."
  type        = string
}

variable "postgres_admin_login" {
  description = "Administrator login for PostgreSQL Flexible Server."
  type        = string
  default     = "unieventadmin"
}

variable "postgres_database_name" {
  description = "Application database name."
  type        = string
  default     = "unievent"
}

variable "postgres_sku_name" {
  description = "Smallest burstable PostgreSQL SKU; verify availability and student-account pricing in the selected region."
  type        = string
  default     = "B_Standard_B1ms"
}

variable "postgres_version" {
  description = "PostgreSQL major version used by the application."
  type        = string
  default     = "16"
}

variable "postgres_storage_mb" {
  description = "Provisioned PostgreSQL storage. 32768 MB is the smallest common Flexible Server allocation."
  type        = number
  default     = 32768
}

variable "container_min_replicas" {
  description = "Keep one API replica running so the existing background automation worker remains active. Set to 0 to scale to zero (scheduled automations will pause while idle)."
  type        = number
  default     = 1

  validation {
    condition     = var.container_min_replicas >= 0 && var.container_min_replicas <= 1
    error_message = "container_min_replicas must be 0 or 1."
  }
}

variable "container_max_replicas" {
  description = "Maximum API replicas. Keep at 1 until scheduled jobs use a distributed lock."
  type        = number
  default     = 1

  validation {
    condition     = var.container_max_replicas >= 1 && var.container_max_replicas <= 2
    error_message = "container_max_replicas must be between 1 and 2."
  }
}

variable "email_smtp_host" {
  description = "SMTP host used by account and certificate email flows."
  type        = string
  default     = "smtp.gmail.com"
}

variable "email_smtp_port" {
  description = "SMTP port."
  type        = number
  default     = 587
}

variable "email_smtp_address" {
  description = "SMTP sender account. Leave empty until email delivery is configured."
  type        = string
  default     = ""
}

variable "email_smtp_password" {
  description = "SMTP password or app password. Stored in Terraform state; protect state and tfvars."
  type        = string
  sensitive   = true
  default     = ""
}

variable "container_app_name" {
  description = "Optional stable API name. Changing an existing name replaces the Container App and changes its URL."
  type        = string
  default     = null

  validation {
    condition     = var.container_app_name == null ? true : can(regex("^[a-z][a-z0-9-]{0,29}[a-z0-9]$", var.container_app_name))
    error_message = "Use 2-31 lowercase letters, numbers or hyphens, starting with a letter and ending with a letter or number."
  }
}
