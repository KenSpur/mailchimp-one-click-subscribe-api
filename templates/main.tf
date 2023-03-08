terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.46.0"
    }
  }
  backend "azurerm" {}
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
  }
}

locals {
  tags = {
    environment = "${var.env}"
    managed_by  = "terraform"
  }
}

# resource group
resource "azurerm_resource_group" "main" {
  name     = "rg-${var.infix}-ocsubapi-${var.env}"
  location = "West Europe"

  tags = local.tags
}