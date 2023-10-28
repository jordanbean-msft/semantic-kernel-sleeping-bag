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

output AZURE_LOCATION string = location
output AZURE_TENANT_ID string = tenant().tenantId
