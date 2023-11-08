targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the the environment which is used to generate a short unique hash used in all resources.')
param environmentName string

@minLength(1)
@description('Primary location for all resources')
param location string

param resourceGroupName string = ''

@description('Id of the user or app to assign application roles')

var abbrs = loadJsonContent('./abbreviations.json')
var resourceToken = toLower(uniqueString(subscription().id, environmentName, location))
var tags = { 'azd-env-name': environmentName }
var containerApps = [
    {
        name: '${abbrs.appContainerApps}historical-weather-lookup'
        daprAppId: 'historical-weather-lookup'
        imageTag: 'historical-weather-lookup'
    }
    {
        name: '${abbrs.appContainerApps}location-lookup'
        daprAppId: 'location-lookup'
        imageTag: 'location-lookup'
    }
    {
        name: '${abbrs.appContainerApps}order-history'
        daprAppId: 'order-history'
        imageTag: 'order-history'
    }
    {
        name: '${abbrs.appContainerApps}product-catalog'
        daprAppId: 'product-catalog'
        imageTag: 'product-catalog'
    }
    {
        name: '${abbrs.appContainerApps}recommendation-api'
        daprAppId: 'recommendation-api'
        imageTag: 'recommendation-api'
    }
    {
        name: '${abbrs.appContainerApps}recommendation-web-app'
        daprAppId: 'recommendation-web-app'
        imageTag: 'recommendation-web-app'
    }
]

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: !empty(resourceGroupName) ? resourceGroupName : '${abbrs.resourcesResourceGroups}${environmentName}'
  location: location
  tags: tags
}

module names 'resource-names.bicep' = {
  scope: az.resourceGroup(resourceGroup.name)
  name: 'resource-names'
  params: {
    resourceToken: resourceToken
  }
}

module loggingDeployment 'logging.bicep' = {
  scope: az.resourceGroup(resourceGroup.name)
  name: 'logging-deployment'
  params: {
    appInsightsName: names.outputs.appInsightsName
    logAnalyticsWorkspaceName: names.outputs.logAnalyticsWorkspaceName
    location: location
    tags: tags
  }
}

module cognitiveServicesDeployment 'cognitive-services.bicep' = {
  scope: az.resourceGroup(resourceGroup.name)
  name: 'cognitive-services-deployment'
  params: {
	cognitiveServicesAccountName: names.outputs.cognitiveServicesAccountName
    cognitiveServicesAccountOpenAiName: names.outputs.cognitiveServicesAcccountOpenAiName
    location: location
	tags: tags
  }
}

module containerRegistryDeployment 'container-registry.bicep' = {
    scope: az.resourceGroup(resourceGroup.name)
    name: 'container-registry-deployment'
    params: {
        containerRegistryName: names.outputs.containerRegistryName
        location: location
        tags: tags
    }
}

module containerAppsEnvironmentDeployment 'container-app-environment.bicep' = {
    scope: az.resourceGroup(resourceGroup.name)
    name: 'container-app-environment-deployment'
    params: {
       containerAppEnvironmentName: names.outputs.containerAppEnvironmentName
       location: location
       tags: tags
    }
}

module containerAppDeployment 'container-app.bicep' = [for containerApp in containerApps: {
    scope: az.resourceGroup(resourceGroup.name)
    name: 'container-app-${containerApp.name}-deployment'
    params: {
        containerRegistryName: containerRegistryDeployment.outputs.containerRegistryName
        containerAppEnvironmentName: containerAppsEnvironmentDeployment.outputs.containerAppEnvironmentName
        containerAppName: containerApp.Name
        daprAppId: containerApp.DaprAppId
        containerAppImageTag: containerApp.ImageTag
        location: location
        tags: tags
    }
}]

output AZURE_LOCATION string = location
output AZURE_TENANT_ID string = tenant().tenantId
