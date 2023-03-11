terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.46.0"
    }
  }
  backend "azurerm" {
    resource_group_name  = "rg"
    storage_account_name = "st"
    container_name       = "mailchimp-one-click-subscribe-api"
    key                  = "terraform.tfstate"
  }
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
    key_vault {
      purge_soft_delete_on_destroy    = true
      recover_soft_deleted_key_vaults = true
    }
  }
}

locals {
  tags = {
    environment = "${var.env_suffix}"
    managed_by  = "terraform"
  }
}

data "azurerm_client_config" "current" {}

# resource group
resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location

  tags = local.tags
}