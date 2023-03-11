resource "azurerm_storage_account" "main" {
  name                = "st${var.org_infix}${var.app_infix}${var.env_suffix}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  tags = local.tags

  account_tier             = "Standard"
  account_replication_type = "LRS"
}