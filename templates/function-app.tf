resource "azurerm_service_plan" "main" {
  name                = "asp-${var.infix}-oneclicksubapi-${var.env}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  tags = local.tags

  os_type  = "Windows"
  sku_name = "Y1"
}

resource "azurerm_windows_function_app" "main" {
  name                = "fn-${var.infix}-oneclicksubapi-${var.env}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  tags = local.tags

  storage_account_name       = azurerm_storage_account.main.name
  storage_account_access_key = azurerm_storage_account.main.primary_access_key
  service_plan_id            = azurerm_service_plan.main.id

  https_only = true

  site_config {
    application_stack {
      dotnet_version              = "v6.0"
      use_dotnet_isolated_runtime = true
    }
  }

  app_settings = {
    FUNCTIONS_EXTENSION_VERSION = "~4"
    FUNCTIONS_WORKER_RUNTIME    = "dotnet-isolated"
  }
}
