resource "azurerm_service_plan" "main" {
  name                = "asp-${var.org_infix}-${var.app_infix}-${var.env_suffix}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  tags = local.tags

  os_type  = "Windows"
  sku_name = "Y1"
}

locals {
  valid_type_keys         = [for type in var.subscription_valid_types : "SubscriptionOptions__ValidTypes__${index(var.subscription_valid_types, type)}"]
  valid-type-settings-map = zipmap(local.valid_type_keys, var.subscription_valid_types)
}

resource "azurerm_windows_function_app" "main" {
  name                = var.function_app_name
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  identity {
    type = "SystemAssigned"
  }

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

  app_settings = merge({
    FUNCTIONS_EXTENSION_VERSION = "~4"
    FUNCTIONS_WORKER_RUNTIME    = "dotnet-isolated"

    # WEBSITE_ENABLE_SYNC_UPDATE_SITE = "true"
    # WEBSITE_RUN_FROM_PACKAGE = "1"

    APPINSIGHTS_INSTRUMENTATIONKEY        = azurerm_application_insights.main.instrumentation_key
    APPLICATIONINSIGHTS_CONNECTION_STRING = azurerm_application_insights.main.connection_string

    ApplicationOptions__RedirectToForm       = "${var.redirect_to_form_url}"
    ApplicationOptions__RedirectToSubscribed = "${var.redirect_to_subscribed_url}"

    HttpOptions__MailchimpApiBaseUrl   = "${var.mailchimp_api_base_url}"
    HttpOptions__MailchimpApiKey       = "@Microsoft.KeyVault(SecretUri=${azurerm_key_vault_secret.mailchimp_api_key.id})"
    HttpOptions__MailchimpAudienceId   = "${var.mailchimp_audience_id}"
    HttpOptions__MailchimpTypeMergeTag = "${var.mailchimp_type_merge_tag}"

    StorageOptions__TableStorageConnectionString = azurerm_storage_account.main.primary_blob_connection_string
    StorageOptions__TableName                    = "${var.subscribers_table_name}"

    SubscriptionOptions__DefaultType = "${var.subscription_default_type}"
  }, local.valid-type-settings-map)
}
