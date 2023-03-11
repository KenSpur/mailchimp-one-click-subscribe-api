# log analytics workspace
resource "azurerm_log_analytics_workspace" "main" {
  name                = "la-${var.org_infix}-${var.app_infix}-${var.env_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = "PerGB2018"
  retention_in_days   = 31
}

# application insights
resource "azurerm_application_insights" "main" {
  name                = "appi-${var.org_infix}-${var.app_infix}-${var.env_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  workspace_id        = azurerm_log_analytics_workspace.main.id
  application_type    = "web"
}