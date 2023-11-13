param name string
param location string = resourceGroup().location
param tags object = {}

@description('The name of the identity')
param identityName string

@description('The name of the Application Insights')
param applicationInsightsName string

@description('The name of the container apps environment')
param containerAppsEnvironmentName string

@description('The name of the container registry')
param containerRegistryName string

@description('The name of the service')
param serviceName string = 'api'

//@description('The name of the image')
//param imageName string = ''

@description('Specifies if the resource exists')
param exists bool

@description('The name of the Key Vault')
param keyVaultName string

@description('The name of the Key Vault resource group')
param keyVaultResourceGroupName string = resourceGroup().name

// @description('The storage blob endpoint')
// param storageBlobEndpoint string

// @description('The name of the storage container')
// param storageContainerName string

// @description('The search service endpoint')
// param searchServiceEndpoint string

// @description('The search index name')
// param searchIndexName string

// @description('The Form Recognizer endpoint')
// param formRecognizerEndpoint string

@description('The OpenAI endpoint')
param openAiEndpoint string

@description('The OpenAI ChatGPT deployment name')
param openAiChatGptDeployment string

@description('The OpenAI Embedding deployment name')
param openAiEmbeddingDeployment string

@description('An array of service binds')
param serviceBinds array

param corsOrigin string = ''

resource webIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: identityName
  location: location
}

module webKeyVaultAccess '../core/security/keyvault-access.bicep' = {
  name: '${serviceName}-keyvault-access'
  scope: resourceGroup(keyVaultResourceGroupName)
  params: {
    principalId: webIdentity.properties.principalId
    keyVaultName: keyVault.name
  }
}

module app '../core/host/container-app-upsert.bicep' = {
  name: '${serviceName}-container-app'
  dependsOn: [ webKeyVaultAccess ]
  params: {
    name: name
    location: location
    tags: union(tags, { 'azd-service-name': 'recommendation-api' })
    identityName: webIdentity.name
    //imageName: imageName
    exists: exists
    serviceBinds: serviceBinds
    containerAppsEnvironmentName: containerAppsEnvironmentName
    containerRegistryName: containerRegistryName
    env: [
      {
        name: 'OpenAI__Endpoint'
        value: openAiEndpoint
      }
      {
        name: 'OpenAI__ChatModelName'
        value: openAiChatGptDeployment
      }
      {
        name: 'OpenAI__EmbeddingModelName'
        value: openAiEmbeddingDeployment
      }
      {
        name: 'ApplicationInsights__ConnectionString'
        value: applicationInsights.properties.ConnectionString
      }
      {
        name: 'EntraID__TenantId'
        value: subscription().tenantId
      }
      {
        name: 'Cors__AllowedOrigins'
        value: corsOrigin
      }
      {
        name: 'AZURE_CLIENT_ID'
        value: webIdentity.properties.clientId
      }
    ]
    targetPort: 8080
    external: true
    daprEnabled: true
    daprAppId: 'recommendation-api'
    daprAppProtocol: 'http'
    containerCpuCoreCount: '1'
    containerMemory: '2.0Gi'
    allowedOrigins: array(corsOrigin)
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' existing = if (!empty(applicationInsightsName)) {
  name: applicationInsightsName
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
  scope: resourceGroup(keyVaultResourceGroupName)
}

output SERVICE_WEB_IDENTITY_NAME string = identityName
output SERVICE_WEB_IDENTITY_PRINCIPAL_ID string = webIdentity.properties.principalId
output SERVICE_WEB_IMAGE_NAME string = app.outputs.imageName
output SERVICE_WEB_NAME string = app.outputs.name
output SERVICE_WEB_URI string = app.outputs.uri

output SERVICE_API_URI string = app.outputs.uri
