resource "random_string" "suffix" {
  length  = 5
  upper   = false
  special = false
}

locals {
  name_prefix  = "${var.project_name}-${random_string.suffix.result}"
  api_app_name = "${local.name_prefix}-api"
  postgres_connection_string = join(";", [
    "Host=${azurerm_postgresql_flexible_server.postgres.fqdn}",
    "Port=5432",
    "Database=${var.postgres_database_name}",
    "Username=${var.postgres_admin_login}",
    "Password=${random_password.postgres.result}",
    "SSL Mode=Require",
    "Trust Server Certificate=true",
    "Timeout=30",
    "Command Timeout=60"
  ])
  web_origin = var.web_url
}

resource "random_password" "postgres" {
  length  = 32
  special = false
}

resource "random_password" "jwt" {
  length           = 64
  special          = true
  override_special = "_-"
}

resource "azurerm_resource_group" "unievent" {
  name     = "${local.name_prefix}-rg"
  location = var.location
}

resource "azurerm_virtual_network" "unievent" {
  name                = "${local.name_prefix}-vnet"
  location            = azurerm_resource_group.unievent.location
  resource_group_name = azurerm_resource_group.unievent.name
  address_space       = ["10.42.0.0/16"]
}

resource "azurerm_subnet" "container_apps" {
  name                 = "container-apps"
  resource_group_name  = azurerm_resource_group.unievent.name
  virtual_network_name = azurerm_virtual_network.unievent.name
  address_prefixes     = ["10.42.0.0/23"]

  delegation {
    name = "container-apps-environment"

    service_delegation {
      name    = "Microsoft.App/environments"
      actions = ["Microsoft.Network/virtualNetworks/subnets/join/action"]
    }
  }
}

resource "azurerm_subnet" "postgres" {
  name                 = "postgres"
  resource_group_name  = azurerm_resource_group.unievent.name
  virtual_network_name = azurerm_virtual_network.unievent.name
  address_prefixes     = ["10.42.2.0/28"]

  delegation {
    name = "postgres-flexible-server"

    service_delegation {
      name    = "Microsoft.DBforPostgreSQL/flexibleServers"
      actions = ["Microsoft.Network/virtualNetworks/subnets/join/action"]
    }
  }
}

resource "azurerm_private_dns_zone" "postgres" {
  name                = "private.postgres.database.azure.com"
  resource_group_name = azurerm_resource_group.unievent.name
}

resource "azurerm_private_dns_zone_virtual_network_link" "postgres" {
  name                  = "${local.name_prefix}-postgres-dns-link"
  private_dns_zone_name = azurerm_private_dns_zone.postgres.name
  resource_group_name   = azurerm_resource_group.unievent.name
  virtual_network_id    = azurerm_virtual_network.unievent.id
}

resource "azurerm_postgresql_flexible_server" "postgres" {
  name                          = "${local.name_prefix}-pg"
  resource_group_name           = azurerm_resource_group.unievent.name
  location                      = azurerm_resource_group.unievent.location
  version                       = var.postgres_version
  administrator_login           = var.postgres_admin_login
  administrator_password        = random_password.postgres.result
  delegated_subnet_id           = azurerm_subnet.postgres.id
  private_dns_zone_id           = azurerm_private_dns_zone.postgres.id
  public_network_access_enabled = false
  sku_name                      = var.postgres_sku_name
  storage_mb                    = var.postgres_storage_mb
  backup_retention_days         = 7
  geo_redundant_backup_enabled  = false
  zone                          = "1"

  authentication {
    active_directory_auth_enabled = false
    password_auth_enabled         = true
  }

  depends_on = [azurerm_private_dns_zone_virtual_network_link.postgres]
}

resource "azurerm_postgresql_flexible_server_database" "unievent" {
  name      = var.postgres_database_name
  server_id = azurerm_postgresql_flexible_server.postgres.id
  charset   = "UTF8"
  collation = "en_US.utf8"
}

resource "azurerm_container_app_environment" "unievent" {
  name                           = "${local.name_prefix}-env"
  location                       = azurerm_resource_group.unievent.location
  resource_group_name            = azurerm_resource_group.unievent.name
  infrastructure_subnet_id       = azurerm_subnet.container_apps.id
  internal_load_balancer_enabled = false
  zone_redundancy_enabled        = false
  log_analytics_workspace_id     = azurerm_log_analytics_workspace.unievent.id
  logs_destination               = "log-analytics"
}

resource "azurerm_log_analytics_workspace" "unievent" {
  name                = "${local.name_prefix}-logs"
  location            = azurerm_resource_group.unievent.location
  resource_group_name = azurerm_resource_group.unievent.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
  daily_quota_gb      = 0.1
}

resource "azurerm_container_app" "api" {
  name                         = local.api_app_name
  container_app_environment_id = azurerm_container_app_environment.unievent.id
  resource_group_name          = azurerm_resource_group.unievent.name
  revision_mode                = "Single"

  # GitHub Actions owns image updates after the initial provisioning.
  lifecycle {
    ignore_changes = [template[0].container[0].image]
  }

  secret {
    name  = "postgres-connection"
    value = local.postgres_connection_string
  }

  secret {
    name  = "jwt-signing-key"
    value = random_password.jwt.result
  }

  dynamic "secret" {
    for_each = var.email_smtp_password == "" ? [] : [var.email_smtp_password]
    content {
      name  = "smtp-password"
      value = secret.value
    }
  }

  template {
    min_replicas = var.container_min_replicas
    max_replicas = var.container_max_replicas

    container {
      name   = "api"
      image  = var.container_image
      cpu    = 0.25
      memory = "0.5Gi"

      env {
        name        = "ConnectionStrings__DefaultConnection"
        secret_name = "postgres-connection"
      }

      env {
        name        = "Jwt__Key"
        secret_name = "jwt-signing-key"
      }

      env {
        name  = "Jwt__Issuer"
        value = "unievent-api"
      }

      env {
        name  = "Jwt__Audience"
        value = "unievent-clients"
      }

      env {
        name  = "Cors__AllowedOrigins__0"
        value = local.web_origin
      }

      env {
        name  = "EmailSettings__BaseUrl"
        value = local.web_origin
      }

      env {
        name  = "EmailSettings__ConfirmationBaseUrl"
        value = "https://${local.api_app_name}.${azurerm_container_app_environment.unievent.default_domain}"
      }

      env {
        name  = "EmailSettings__WebLoginUrl"
        value = "${local.web_origin}/login"
      }

      env {
        name  = "EmailSettings__MobileLoginDeepLink"
        value = "unievent://login"
      }

      env {
        name  = "EmailSettings__Host"
        value = var.email_smtp_host
      }

      env {
        name  = "EmailSettings__Port"
        value = tostring(var.email_smtp_port)
      }

      env {
        name  = "EmailSettings__Email"
        value = var.email_smtp_address
      }

      env {
        name  = "EmailSettings__EnableSsl"
        value = "true"
      }

      dynamic "env" {
        for_each = var.email_smtp_password == "" ? [] : [var.email_smtp_password]
        content {
          name        = "EmailSettings__Password"
          secret_name = "smtp-password"
        }
      }

      env {
        name  = "Database__MigrateOnStartup"
        value = "true"
      }

      env {
        name  = "Automacoes__Ativas"
        value = "true"
      }

      env {
        name  = "ASPNETCORE_ENVIRONMENT"
        value = "Production"
      }

      env {
        name  = "ASPNETCORE_URLS"
        value = "http://+:8080"
      }
    }

    http_scale_rule {
      name                = "http"
      concurrent_requests = "50"
    }
  }

  ingress {
    external_enabled           = true
    allow_insecure_connections = false
    target_port                = 8080
    transport                  = "auto"

    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }

  depends_on = [azurerm_postgresql_flexible_server_database.unievent]
}
