targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the the environment which is used to generate a short unique hash used in all resources.')
param environmentName string

@minLength(1)
@description('Primary location for all resources')
param location string
param tags string = ''

// Optional parameters to override the default azd resource naming conventions. Update the main.parameters.json file to provide values. e.g.,:
// "resourceGroupName": {
//      "value": "myGroupName"
// }
param historicalWeatherLookupContainerAppName string = ''
//param historicalWeatherLookupImageName string = ''
param historicalWeatherLookupIdentityName string = ''
param locationLookupContainerAppName string = ''
//param locationLookupImageName string = ''
param locationLookupIdentityName string = ''
param orderHistoryContainerAppName string = ''
//param orderHistoryImageName string = ''
param orderHistoryIdentityName string = ''
param productCatalogContainerAppName string = ''
//param productCatalogImageName string = ''
param productCatalogIdentityName string = ''
param recommendationApiContainerAppName string = ''
//param recommendationApiImageName string = ''
param recommendationApiIdentityName string = ''
param recommendationWebAppContainerAppName string = ''
//param recommendationWebAppImageName string = ''
param recommendationWebAppIdentityName string = ''
param historicalWeatherLookupAppExists bool = false
param locationLookupAppExists bool = false
param orderHistoryAppExists bool = false
param productCatalogAppExists bool = false
param recommendationApiAppExists bool = false
param recommendationWebAppExists bool = false

@description('Location for the OpenAI resource group')
@allowed([ 'canadaeast', 'eastus', 'eastus2', 'francecentral', 'switzerlandnorth', 'uksouth', 'japaneast', 'northcentralus', 'australiaeast' ])
@metadata({
  azd: {
    type: 'location'
  }
})
param openAiResourceGroupLocation string

@description('Name of the chat GPT model. Default: gpt-35-turbo')
@allowed([ 'gpt-35-turbo', 'gpt-4', 'gpt-35-turbo-16k', 'gpt-4-16k' ])
param chatGptModelName string = 'gpt-35-turbo'

@description('Name of the Azure Application Insights dashboard')
param applicationInsightsDashboardName string = ''

@description('Name of the Azure Application Insights resource')
param applicationInsightsName string = ''

@description('Name of the Azure App Service Plan')
param appServicePlanName string = ''

@description('Capacity of the chat GPT deployment. Default: 30')
param chatGptDeploymentCapacity int = 30

@description('Name of the chat completion deployment')
param chatCompletionDeploymentName string = 'chat'

@description('ID of the chat completion model')
param chatCompletionModelId string = '0613'

@description('Name of the embedding deployment. Default: embedding')
param embeddingDeploymentName string = 'embedding'

@description('ID of the embedding model')
param embeddingModelId string = '2'

@description('Whether or not to use an API key for the OpenAI service. Default: false')
param openAiUseApiKey bool = false

@description('Capacity of the embedding deployment. Default: 30')
param embeddingDeploymentCapacity int = 30

@description('Name of the embedding model. Default: text-embedding-ada-002')
param embeddingModelName string = 'text-embedding-ada-002'

@description('Name of the container apps environment')
param containerAppsEnvironmentName string = ''

@description('Name of the Azure container registry')
param containerRegistryName string = ''

@description('Name of the resource group for the Azure container registry')
param containerRegistryResourceGroupName string = ''

@description('Location of the resource group for the Form Recognizer service')
param formRecognizerResourceGroupLocation string = location

@description('Name of the resource group for the Form Recognizer service')
param formRecognizerResourceGroupName string = ''

@description('Name of the Form Recognizer service')
param formRecognizerServiceName string = ''

@description('SKU name for the Form Recognizer service. Default: S0')
param formRecognizerSkuName string = 'S0'

@description('Name of the Azure Function App')
param functionServiceName string = ''

@description('Name of the Azure Key Vault')
param keyVaultName string = ''

@description('Location of the resource group for the Azure Key Vault')
param keyVaultResourceGroupLocation string = location

@description('Name of the resource group for the Azure Key Vault')
param keyVaultResourceGroupName string = ''

@description('Name of the Azure Log Analytics workspace')
param logAnalyticsName string = ''

@description('Name of the resource group for the OpenAI resources')
param openAiResourceGroupName string = ''

@description('Name of the OpenAI service')
param openAiServiceName string = ''

@description('SKU name for the OpenAI service. Default: S0')
param openAiSkuName string = 'S0'

@description('ID of the principal')
param principalId string = ''

@description('Type of the principal. Valid values: User,ServicePrincipal')
param principalType string = 'User'

@description('Name of the resource group')
param resourceGroupName string = ''

@description('Name of the search index. Default: gptkbindex')
param searchIndexName string = 'gptkbindex'

@description('Name of the Azure Cognitive Search service')
param searchServiceName string = ''

@description('Location of the resource group for the Azure Cognitive Search service')
param searchServiceResourceGroupLocation string = location

@description('Name of the resource group for the Azure Cognitive Search service')
param searchServiceResourceGroupName string = ''

@description('SKU name for the Azure Cognitive Search service. Default: standard')
param searchServiceSkuName string = 'standard'

@description('Name of the storage account')
param storageAccountName string = ''

@description('Name of the storage container. Default: content')
param storageContainerName string = 'content'

@description('Location of the resource group for the storage account')
param storageResourceGroupLocation string = location

@description('Name of the resource group for the storage account')
param storageResourceGroupName string = ''

@description('The base URL used by the web service for sending API requests')
param webApiBaseUrl string = ''

var abbrs = loadJsonContent('./abbreviations.json')
var resourceToken = toLower(uniqueString(subscription().id, environmentName, location))
var baseTags = { 'azd-env-name': environmentName }
var updatedTags = union(empty(tags) ? {} : base64ToJson(tags), baseTags)
var webContainerAppNameOrDefault = '${abbrs.appContainerApps}web-app-${resourceToken}'
var corsAcaUrl = 'https://${webContainerAppNameOrDefault}.${containerApps.outputs.defaultDomain}'

// Organize resources in a resource group
resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: !empty(resourceGroupName) ? resourceGroupName : '${abbrs.resourcesResourceGroups}${environmentName}'
  location: location
  tags: baseTags
}

resource openAiResourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' existing = if (!empty(openAiResourceGroupName)) {
  name: !empty(openAiResourceGroupName) ? openAiResourceGroupName : resourceGroup.name
}

resource formRecognizerResourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' existing = if (!empty(formRecognizerResourceGroupName)) {
  name: !empty(formRecognizerResourceGroupName) ? formRecognizerResourceGroupName : resourceGroup.name
}

resource searchServiceResourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' existing = if (!empty(searchServiceResourceGroupName)) {
  name: !empty(searchServiceResourceGroupName) ? searchServiceResourceGroupName : resourceGroup.name
}

resource storageResourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' existing = if (!empty(storageResourceGroupName)) {
  name: !empty(storageResourceGroupName) ? storageResourceGroupName : resourceGroup.name
}

resource keyVaultResourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' existing = if (!empty(keyVaultResourceGroupName)) {
  name: !empty(keyVaultResourceGroupName) ? keyVaultResourceGroupName : resourceGroup.name
}

// Container apps host (including container registry)
module containerApps './core/host/container-apps.bicep' = {
  name: 'container-apps'
  scope: resourceGroup
  params: {
    name: 'app'
    location: location
    tags: baseTags
    containerAppsEnvironmentName: !empty(containerAppsEnvironmentName) ? containerAppsEnvironmentName : '${abbrs.appManagedEnvironments}${resourceToken}'
    containerRegistryName: !empty(containerRegistryName) ? containerRegistryName : '${abbrs.containerRegistryRegistries}${resourceToken}'
    logAnalyticsWorkspaceName: monitoring.outputs.logAnalyticsWorkspaceName
    applicationInsightsName: monitoring.outputs.applicationInsightsName
  }
}

// Web frontend
module web './app/recommendation-web-app.bicep' = {
  name: 'web-app'
  scope: resourceGroup
  params: {
    name: !empty(recommendationWebAppContainerAppName) ? recommendationWebAppContainerAppName : '${abbrs.appContainerApps}web-app-${resourceToken}'
    location: location
    tags: updatedTags
    //imageName: recommendationWebAppImageName
    identityName: !empty(recommendationWebAppIdentityName) ? recommendationWebAppIdentityName : '${abbrs.managedIdentityUserAssignedIdentities}web-app-${resourceToken}'
    applicationInsightsName: monitoring.outputs.applicationInsightsName
    containerAppsEnvironmentName: containerApps.outputs.environmentName
    containerRegistryName: containerApps.outputs.registryName
    exists: recommendationWebAppExists
    keyVaultName: keyVault.outputs.name
    keyVaultResourceGroupName: keyVaultResourceGroup.name
    //storageBlobEndpoint: storage.outputs.primaryEndpoints.blob
    //storageContainerName: storageContainerName
    //searchServiceEndpoint: searchService.outputs.endpoint
    //searchIndexName: searchIndexName
    //formRecognizerEndpoint: formRecognizer.outputs.endpoint
    //openAiEndpoint: openAi.outputs.endpoint
    //openAiChatGptDeployment: chatGptDeploymentName
    //openAiEmbeddingDeployment: embeddingDeploymentName
    serviceBinds: []
    apiBaseUrl: !empty(webApiBaseUrl) ? webApiBaseUrl : api.outputs.SERVICE_API_URI
  }
}

// Api backend
module api './app/recommendation-api.bicep' = {
  name: 'api'
  scope: resourceGroup
  params: {
    name: !empty(recommendationApiContainerAppName) ? recommendationApiContainerAppName : '${abbrs.appContainerApps}api-${resourceToken}'
    location: location
    tags: updatedTags
    //imageName: recommendationApiImageName
    identityName: !empty(recommendationApiIdentityName) ? recommendationApiIdentityName : '${abbrs.managedIdentityUserAssignedIdentities}api-${resourceToken}'
    applicationInsightsName: monitoring.outputs.applicationInsightsName
    containerAppsEnvironmentName: containerApps.outputs.environmentName
    containerRegistryName: containerApps.outputs.registryName
    exists: recommendationApiAppExists
    keyVaultName: keyVault.outputs.name
    keyVaultResourceGroupName: keyVaultResourceGroup.name
    //storageBlobEndpoint: storage.outputs.primaryEndpoints.blob
    //storageContainerName: storageContainerName
    //searchServiceEndpoint: searchService.outputs.endpoint
    //searchIndexName: searchIndexName
    //formRecognizerEndpoint: formRecognizer.outputs.endpoint
    openAiEndpoint: openAi.outputs.endpoint
    openAiChatCompletionDeploymentName: chatCompletionDeploymentName
    openAiChatCompletionModelId: chatCompletionModelId
    openAiEmbeddingDeploymentName: embeddingDeploymentName
    openAiEmbeddingModelId: embeddingModelId
    openAiUseApiKey: openAiUseApiKey
    serviceBinds: []
    corsOrigin: corsAcaUrl
  }
}

module historicalWeatherLookupApi './app/historical-weather-lookup.bicep' = {
  name: 'weather-lookup'
  scope: resourceGroup
  params: {
    name: !empty(historicalWeatherLookupContainerAppName) ? historicalWeatherLookupContainerAppName : '${abbrs.appContainerApps}weather-lookup-${resourceToken}'
    location: location
    tags: updatedTags
    //imageName: historicalWeatherLookupImageName
    identityName: !empty(historicalWeatherLookupIdentityName) ? historicalWeatherLookupIdentityName : '${abbrs.managedIdentityUserAssignedIdentities}weather-lookup-${resourceToken}'
    applicationInsightsName: monitoring.outputs.applicationInsightsName
    containerAppsEnvironmentName: containerApps.outputs.environmentName
    containerRegistryName: containerApps.outputs.registryName
    exists: historicalWeatherLookupAppExists
    keyVaultName: keyVault.outputs.name
    keyVaultResourceGroupName: keyVaultResourceGroup.name
    //storageBlobEndpoint: storage.outputs.primaryEndpoints.blob
    //storageContainerName: storageContainerName
    //searchServiceEndpoint: searchService.outputs.endpoint
    //searchIndexName: searchIndexName
    //formRecognizerEndpoint: formRecognizer.outputs.endpoint
    //openAiEndpoint: openAi.outputs.endpoint
    //openAiChatGptDeployment: chatGptDeploymentName
    //openAiEmbeddingDeployment: embeddingDeploymentName
    serviceBinds: []
  }
}

module locationLookupApi './app/location-lookup.bicep' = {
  name: 'location-lookup'
  scope: resourceGroup
  params: {
    name: !empty(locationLookupContainerAppName) ? locationLookupContainerAppName : '${abbrs.appContainerApps}location-lookup-${resourceToken}'
    location: location
    tags: updatedTags
    //imageName: locationLookupImageName
    identityName: !empty(locationLookupIdentityName) ? locationLookupIdentityName : '${abbrs.managedIdentityUserAssignedIdentities}location-lookup-${resourceToken}'
    applicationInsightsName: monitoring.outputs.applicationInsightsName
    containerAppsEnvironmentName: containerApps.outputs.environmentName
    containerRegistryName: containerApps.outputs.registryName
    exists: locationLookupAppExists
    keyVaultName: keyVault.outputs.name
    keyVaultResourceGroupName: keyVaultResourceGroup.name
    //storageBlobEndpoint: storage.outputs.primaryEndpoints.blob
    //storageContainerName: storageContainerName
    //searchServiceEndpoint: searchService.outputs.endpoint
    //searchIndexName: searchIndexName
    //formRecognizerEndpoint: formRecognizer.outputs.endpoint
    //openAiEndpoint: openAi.outputs.endpoint
    //openAiChatGptDeployment: chatGptDeploymentName
    //openAiEmbeddingDeployment: embeddingDeploymentName
    serviceBinds: []
  }
}

module orderHistoryApi './app/order-history.bicep' = {
  name: 'order-history'
  scope: resourceGroup
  params: {
    name: !empty(orderHistoryContainerAppName) ? orderHistoryContainerAppName : '${abbrs.appContainerApps}order-history-${resourceToken}'
    location: location
    tags: updatedTags
    //imageName: orderHistoryImageName
    identityName: !empty(orderHistoryIdentityName) ? orderHistoryIdentityName : '${abbrs.managedIdentityUserAssignedIdentities}order-history-${resourceToken}'
    applicationInsightsName: monitoring.outputs.applicationInsightsName
    containerAppsEnvironmentName: containerApps.outputs.environmentName
    containerRegistryName: containerApps.outputs.registryName
    exists: orderHistoryAppExists
    keyVaultName: keyVault.outputs.name
    keyVaultResourceGroupName: keyVaultResourceGroup.name
    //storageBlobEndpoint: storage.outputs.primaryEndpoints.blob
    //storageContainerName: storageContainerName
    //searchServiceEndpoint: searchService.outputs.endpoint
    //searchIndexName: searchIndexName
    //formRecognizerEndpoint: formRecognizer.outputs.endpoint
    //openAiEndpoint: openAi.outputs.endpoint
    //openAiChatGptDeployment: chatGptDeploymentName
    //openAiEmbeddingDeployment: embeddingDeploymentName
    serviceBinds: []
  }
}

module productCatalogApi './app/product-catalog.bicep' = {
  name: 'product-catalog'
  scope: resourceGroup
  params: {
    name: !empty(productCatalogContainerAppName) ? productCatalogContainerAppName : '${abbrs.appContainerApps}product-catalog-${resourceToken}'
    location: location
    tags: updatedTags
    //imageName: productCatalogImageName
    identityName: !empty(productCatalogIdentityName) ? productCatalogIdentityName : '${abbrs.managedIdentityUserAssignedIdentities}product-catalog-${resourceToken}'
    applicationInsightsName: monitoring.outputs.applicationInsightsName
    containerAppsEnvironmentName: containerApps.outputs.environmentName
    containerRegistryName: containerApps.outputs.registryName
    exists: productCatalogAppExists
    keyVaultName: keyVault.outputs.name
    keyVaultResourceGroupName: keyVaultResourceGroup.name
    //storageBlobEndpoint: storage.outputs.primaryEndpoints.blob
    //storageContainerName: storageContainerName
    //searchServiceEndpoint: searchService.outputs.endpoint
    //searchIndexName: searchIndexName
    //formRecognizerEndpoint: formRecognizer.outputs.endpoint
    //openAiEndpoint: openAi.outputs.endpoint
    //openAiChatGptDeployment: chatGptDeploymentName
    //openAiEmbeddingDeployment: embeddingDeploymentName
    serviceBinds: []
  }
}

module openAi 'core/ai/cognitiveservices.bicep' = {
  name: 'openai'
  scope: openAiResourceGroup
  params: {
    name: !empty(openAiServiceName) ? openAiServiceName : '${abbrs.cognitiveServicesAccounts}${resourceToken}'
    location: openAiResourceGroupLocation
    tags: updatedTags
    sku: {
      name: openAiSkuName
    }
    deployments: [
      {
        name: chatCompletionDeploymentName
        model: {
          format: 'OpenAI'
          name: chatGptModelName
          version: chatCompletionModelId
        }
        sku: {
          name: 'Standard'
          capacity: chatGptDeploymentCapacity
        }
      }
      {
        name: embeddingDeploymentName
        model: {
          format: 'OpenAI'
          name: embeddingModelName
          version: embeddingModelId
        }
        sku: {
          name: 'Standard'
          capacity: embeddingDeploymentCapacity
        }
      }
    ]
  }
}

// Store secrets in a keyvault
module keyVault './core/security/keyvault.bicep' = {
  name: 'keyvault'
  scope: resourceGroup
  params: {
    name: !empty(keyVaultName) ? keyVaultName : '${abbrs.keyVaultVaults}${resourceToken}'
    location: location
    tags: baseTags
    principalId: principalId
  }
}

// Monitor application with Azure Monitor
module monitoring './core/monitor/monitoring.bicep' = {
  name: 'monitoring'
  scope: resourceGroup
  params: {
    location: location
    tags: baseTags
    logAnalyticsName: !empty(logAnalyticsName) ? logAnalyticsName : '${abbrs.operationalInsightsWorkspaces}${resourceToken}'
    applicationInsightsName: !empty(applicationInsightsName) ? applicationInsightsName : '${abbrs.insightsComponents}${resourceToken}'
    applicationInsightsDashboardName: !empty(applicationInsightsDashboardName) ? applicationInsightsDashboardName : '${abbrs.portalDashboards}${resourceToken}'
  }
}

// USER ROLES
module openAiRoleUser 'core/security/role.bicep' = {
  scope: openAiResourceGroup
  name: 'openai-role-user'
  params: {
    principalId: principalId
    roleDefinitionId: '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd'
    principalType: principalType
  }
}

module formRecognizerRoleUser 'core/security/role.bicep' = {
  scope: formRecognizerResourceGroup
  name: 'formrecognizer-role-user'
  params: {
    principalId: principalId
    roleDefinitionId: 'a97b65f3-24c7-4388-baec-2e87135dc908'
    principalType: principalType
  }
}

module storageRoleUser 'core/security/role.bicep' = {
  scope: storageResourceGroup
  name: 'storage-role-user'
  params: {
    principalId: principalId
    roleDefinitionId: '2a2b9908-6ea1-4ae2-8e65-a410df84e7d1'
    principalType: principalType
  }
}

module storageContribRoleUser 'core/security/role.bicep' = {
  scope: storageResourceGroup
  name: 'storage-contribrole-user'
  params: {
    principalId: principalId
    roleDefinitionId: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
    principalType: principalType
  }
}

module searchRoleUser 'core/security/role.bicep' = {
  scope: searchServiceResourceGroup
  name: 'search-role-user'
  params: {
    principalId: principalId
    roleDefinitionId: '1407120a-92aa-4202-b7e9-c0e197c71c8f'
    principalType: principalType
  }
}

module searchContribRoleUser 'core/security/role.bicep' = {
  scope: searchServiceResourceGroup
  name: 'search-contrib-role-user'
  params: {
    principalId: principalId
    roleDefinitionId: '8ebe5a00-799e-43f5-93ac-243d3dce84a7'
    principalType: principalType
  }
}

module searchSvcContribRoleUser 'core/security/role.bicep' = {
  scope: searchServiceResourceGroup
  name: 'search-svccontrib-role-user'
  params: {
    principalId: principalId
    roleDefinitionId: '7ca78c08-252a-4471-8644-bb5ff32d4ba0'
    principalType: principalType
  }
}

// FUNCTION ROLES
// module openAiRoleFunction 'core/security/role.bicep' = {
//   scope: openAiResourceGroup
//   name: 'openai-role-function'
//   params: {
//     principalId: function.outputs.SERVICE_FUNCTION_IDENTITY_PRINCIPAL_ID
//     roleDefinitionId: '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd'
//     principalType: 'ServicePrincipal'
//   }
// }

// module formRecognizerRoleFunction 'core/security/role.bicep' = {
//   scope: formRecognizerResourceGroup
//   name: 'formrecognizer-role-function'
//   params: {
//     principalId: function.outputs.SERVICE_FUNCTION_IDENTITY_PRINCIPAL_ID
//     roleDefinitionId: 'a97b65f3-24c7-4388-baec-2e87135dc908'
//     principalType: 'ServicePrincipal'
//   }
// }

// module storageRoleFunction 'core/security/role.bicep' = {
//   scope: storageResourceGroup
//   name: 'storage-role-function'
//   params: {
//     principalId: function.outputs.SERVICE_FUNCTION_IDENTITY_PRINCIPAL_ID
//     roleDefinitionId: '2a2b9908-6ea1-4ae2-8e65-a410df84e7d1'
//     principalType: 'ServicePrincipal'
//   }
// }

// module storageContribRoleFunction 'core/security/role.bicep' = {
//   scope: storageResourceGroup
//   name: 'storage-contribrole-function'
//   params: {
//     principalId: function.outputs.SERVICE_FUNCTION_IDENTITY_PRINCIPAL_ID
//     roleDefinitionId: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
//     principalType: 'ServicePrincipal'
//   }
// }

// module searchRoleFunction 'core/security/role.bicep' = {
//   scope: searchServiceResourceGroup
//   name: 'search-role-function'
//   params: {
//     principalId: function.outputs.SERVICE_FUNCTION_IDENTITY_PRINCIPAL_ID
//     roleDefinitionId: '1407120a-92aa-4202-b7e9-c0e197c71c8f'
//     principalType: 'ServicePrincipal'
//   }
// }

// module searchContribRoleFunction 'core/security/role.bicep' = {
//   scope: searchServiceResourceGroup
//   name: 'search-contrib-role-function'
//   params: {
//     principalId: function.outputs.SERVICE_FUNCTION_IDENTITY_PRINCIPAL_ID
//     roleDefinitionId: '8ebe5a00-799e-43f5-93ac-243d3dce84a7'
//     principalType: 'ServicePrincipal'
//   }
// }

// module searchSvcContribRoleFunction 'core/security/role.bicep' = {
//   scope: searchServiceResourceGroup
//   name: 'search-svccontrib-role-function'
//   params: {
//     principalId: function.outputs.SERVICE_FUNCTION_IDENTITY_PRINCIPAL_ID
//     roleDefinitionId: '7ca78c08-252a-4471-8644-bb5ff32d4ba0'
//     principalType: 'ServicePrincipal'
//   }
// }

// SYSTEM IDENTITIES
module openAiRoleBackend 'core/security/role.bicep' = {
  scope: openAiResourceGroup
  name: 'openai-role-backend'
  params: {
    principalId: api.outputs.SERVICE_WEB_IDENTITY_PRINCIPAL_ID
    roleDefinitionId: '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd'
    principalType: 'ServicePrincipal'
  }
}

module storageRoleBackend 'core/security/role.bicep' = {
  scope: storageResourceGroup
  name: 'storage-role-backend'
  params: {
    principalId: api.outputs.SERVICE_WEB_IDENTITY_PRINCIPAL_ID
    roleDefinitionId: '2a2b9908-6ea1-4ae2-8e65-a410df84e7d1'
    principalType: 'ServicePrincipal'
  }
}

module storageContribRoleBackend 'core/security/role.bicep' = {
  scope: storageResourceGroup
  name: 'storage-contribrole-backend'
  params: {
    principalId: api.outputs.SERVICE_WEB_IDENTITY_PRINCIPAL_ID
    roleDefinitionId: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
    principalType: 'ServicePrincipal'
  }
}

module searchRoleBackend 'core/security/role.bicep' = {
  scope: searchServiceResourceGroup
  name: 'search-role-backend'
  params: {
    principalId: api.outputs.SERVICE_WEB_IDENTITY_PRINCIPAL_ID
    roleDefinitionId: '1407120a-92aa-4202-b7e9-c0e197c71c8f'
    principalType: 'ServicePrincipal'
  }
}

output APPLICATIONINSIGHTS_CONNECTION_STRING string = monitoring.outputs.applicationInsightsConnectionString
output APPLICATIONINSIGHTS_NAME string = monitoring.outputs.applicationInsightsName
output AZURE_CONTAINER_ENVIRONMENT_NAME string = containerApps.outputs.environmentName
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = containerApps.outputs.registryLoginServer
output AZURE_CONTAINER_REGISTRY_NAME string = containerApps.outputs.registryName
output AZURE_CONTAINER_REGISTRY_RESOURCE_GROUP string = containerApps.outputs.registryName
output AZURE_KEY_VAULT_ENDPOINT string = keyVault.outputs.endpoint
output AZURE_KEY_VAULT_NAME string = keyVault.outputs.name
output AZURE_KEY_VAULT_RESOURCE_GROUP string = keyVaultResourceGroup.name
output AZURE_LOCATION string = location
output AZURE_OPENAI_RESOURCE_LOCATION string = openAiResourceGroupLocation
output AZURE_OPENAI_CHATCOMPLETION_DEPLOYMENT string = chatCompletionDeploymentName
output AZURE_OPENAI_CHATCOMPLETION_MODEL_ID string = chatCompletionModelId
output AZURE_OPENAI_EMBEDDING_DEPLOYMENT string = embeddingDeploymentName
output AZURE_OPENAI_EMBEDDING_MODEL_ID string = embeddingModelId
output AZURE_OPENAI_USE_API_KEY bool = openAiUseApiKey
output AZURE_OPENAI_ENDPOINT string = openAi.outputs.endpoint
output AZURE_OPENAI_RESOURCE_GROUP string = openAiResourceGroup.name
output AZURE_OPENAI_SERVICE string = openAi.outputs.name
// output AZURE_REDIS_CACHE string = redis.outputs.name
output AZURE_RESOURCE_GROUP string = resourceGroup.name
output AZURE_SEARCH_INDEX string = searchIndexName
output AZURE_STORAGE_RESOURCE_GROUP string = storageResourceGroup.name
output AZURE_TENANT_ID string = tenant().tenantId
output SERVICE_RECOMMENDATION_WEB_APP_IDENTITY_NAME string = web.outputs.SERVICE_WEB_IDENTITY_NAME
output SERVICE_RECOMMENDATION_WEB_APP_NAME string = web.outputs.SERVICE_WEB_NAME
output SERVICE_RECOMMENDATION_API_APP_IDENTITY_NAME string = api.outputs.SERVICE_WEB_IDENTITY_NAME
output SERVICE_RECOMMENDATION_API_APP_NAME string = api.outputs.SERVICE_WEB_NAME
output SERVICE_HISTORICAL_WEATHER_LOOKUP_APP_IDENTITY_NAME string = historicalWeatherLookupApi.outputs.SERVICE_WEB_IDENTITY_NAME
output SERVICE_HISTORICAL_WEATHER_LOOKUP_APP_NAME string = historicalWeatherLookupApi.outputs.SERVICE_WEB_NAME
output SERVICE_LOCATION_LOOKUP_APP_IDENTITY_NAME string = locationLookupApi.outputs.SERVICE_WEB_IDENTITY_NAME
output SERVICE_LOCATION_LOOKUP_APP_NAME string = locationLookupApi.outputs.SERVICE_WEB_NAME
output SERVICE_ORDER_HISTORY_APP_IDENTITY_NAME string = orderHistoryApi.outputs.SERVICE_WEB_IDENTITY_NAME
output SERVICE_ORDER_HISTORY_APP_NAME string = orderHistoryApi.outputs.SERVICE_WEB_NAME
output SERVICE_PRODUCT_CATALOG_APP_IDENTITY_NAME string = productCatalogApi.outputs.SERVICE_WEB_IDENTITY_NAME
output SERVICE_PRODUCT_CATALOG_APP_NAME string = productCatalogApi.outputs.SERVICE_WEB_NAME
