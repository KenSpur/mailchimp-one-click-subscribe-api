resource "azurerm_key_vault" "main" {
  name                = "kv${var.org_infix}${var.app_infix}${var.env_suffix}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  tenant_id           = data.azurerm_client_config.current.tenant_id

  tags = local.tags

  soft_delete_retention_days = 7
  purge_protection_enabled   = false

  sku_name = "standard"

  enable_rbac_authorization = true
}

resource "azurerm_role_assignment" "key_vault_secret_officer" {
  scope                = azurerm_key_vault.main.id
  role_definition_name = "Key Vault Secrets Officer"
  principal_id         = data.azurerm_client_config.current.object_id
}

resource "azurerm_key_vault_secret" "mailchimp_api_key" {
  name         = "mailchimp-api-key"
  value        = var.mailchimp_api_key
  key_vault_id = azurerm_key_vault.main.id

  depends_on = [
    azurerm_role_assignment.key_vault_secret_officer
  ]
}

resource "azurerm_role_assignment" "key_vault_secret_user" {
  scope                = azurerm_key_vault.main.id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = azurerm_windows_function_app.main.identity[0].principal_id
}