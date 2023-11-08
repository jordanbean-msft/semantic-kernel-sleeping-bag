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
param historicalWeatherLookupImageName string = ''
param historicalWeatherLookupIdentityName string = ''
param locationLookupContainerAppName string = ''
param locationLookupImageName string = ''
param locationLookupIdentityName string = ''
param orderHistoryContainerAppName string = ''
param orderHistoryImageName string = ''
param orderHistoryIdentityName string = ''
param productCatalogContainerAppName string = ''
param productCatalogImageName string = ''
param productCatalogIdentityName string = ''
param recommendationApiContainerAppName string = ''
param recommendationApiImageName string = ''
param recommendationApiIdentityName string = ''
param recommendationWebAppContainerAppName string = ''
param recommendationWebAppImageName string = ''
param recommendationWebAppIdentityName string = ''
param historicalWeatherLookupAppExists bool = false
param locationLookupAppExists bool = false
param orderHistoryAppExists bool = false
param productCatalogAppExists bool = false
param recommendationApiAppExists bool = false
param recommendationWebAppExists bool = false

@description('Location for the OpenAI resource group')
@allowed([ 'canadaeast', 'eastus', 'eastus2', 'francecentral', 'switzerlandnorth', 'uksouth', 'japaneast', 'northcentralus' ])
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

@description('Name of the chat GPT deployment')
param chatGptDeploymentName string = 'chat'

@description('Name of the embedding deployment. Default: embedding')
param embeddingDeploymentName string = 'embedding'

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
var apiContainerAppNameOrDefault = '${abbrs.appContainerApps}web-${resourceToken}'
var corsAcaUrl = 'https://${apiContainerAppNameOrDefault}.${containerApps.outputs.defaultDomain}'

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
    imageName: recommendationWebAppImageName
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
    imageName: recommendationApiImageName
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
    //openAiEndpoint: openAi.outputs.endpoint
    //openAiChatGptDeployment: chatGptDeploymentName
    //openAiEmbeddingDeployment: embeddingDeploymentName
    serviceBinds: []
  }
}

module historicalWeatherLookupApi './app/historical-weather-lookup.bicep' = {
  name: 'weather-lookup'
  scope: resourceGroup
  params: {
    name: !empty(historicalWeatherLookupContainerAppName) ? historicalWeatherLookupContainerAppName : '${abbrs.appContainerApps}weather-lookup-${resourceToken}'
    location: location
    tags: updatedTags
    imageName: historicalWeatherLookupImageName
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
    imageName: locationLookupImageName
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
    imageName: orderHistoryImageName
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
    imageName: productCatalogImageName
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
        name: chatGptDeploymentName
        model: {
          format: 'OpenAI'
          name: chatGptModelName
          version: '0301'
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
          version: '2'
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
output AZURE_OPENAI_CHATGPT_DEPLOYMENT string = chatGptDeploymentName
output AZURE_OPENAI_EMBEDDING_DEPLOYMENT string = embeddingDeploymentName
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
