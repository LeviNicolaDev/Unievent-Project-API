output "api_url" {
  description = "Public HTTPS URL for the API; configure VITE_API_BASE_URL and EXPO_PUBLIC_API_URL with this value."
  value       = "https://${azurerm_container_app.api.ingress[0].fqdn}"
}

output "web_url" {
  description = "Azure Static Web Apps URL."
  value       = "https://${azurerm_static_web_app.web.default_host_name}"
}

output "web_deployment_token" {
  description = "Deployment token for the static web app. Add it as a protected CI secret."
  value       = azurerm_static_web_app.web.api_key
  sensitive   = true
}

output "postgres_server_fqdn" {
  description = "Private PostgreSQL hostname, reachable only from the virtual network."
  value       = azurerm_postgresql_flexible_server.postgres.fqdn
}

output "resource_group_name" {
  description = "Resource group containing the UniEvent Azure resources."
  value       = azurerm_resource_group.unievent.name
}

output "container_app_name" {
  description = "Container App name for subsequent image updates."
  value       = azurerm_container_app.api.name
}
