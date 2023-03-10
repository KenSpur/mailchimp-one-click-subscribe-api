# Naming
variable "org_infix" {
  type    = string
  default = "example"
}

variable "app_infix" {
  type    = string
  default = "example"
}

variable "env" {
  type    = string
  default = "tst"
}

variable "location" {
  type    = string
  default = "West Europe"
}

variable "resource_group_name" {
  type    = string
  default = "rg"
}

variable "function_app_name" {
  type    = string
  default = "fn-app"
}

# Application
variable "redirect_to_form_url" {
  type    = string
  default = "https://example.com"
}

variable "redirect_to_subscribed_url" {
  type    = string
  default = "https://example.com"
}

# Storage
variable "subscribers_table_name" {
  type    = string
  default = "subscribers"
}

# Subscription
variable "subscription_default_type" {
  type    = string
  default = ""
}

variable "subscription_valid_types" {
  type    = list(string)
  default = []
}

# Mailchimp
variable "mailchimp_base_url" {
  type    = string
  default = "mailchimp_base_url"
}

variable "mailchimp_api_key" {
  type      = string
  sensitive = true
  default   = "mailchimp_api_key"
}

variable "mailchimp_audience_id" {
  type    = string
  default = "mailchimp_audience_id"
}

variable "mailchimp_type_merge_tag" {
  type    = string
  default = "mailchimp_type_merge_tag"
}