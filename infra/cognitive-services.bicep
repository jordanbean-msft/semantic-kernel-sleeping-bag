param cognitiveServicesAccountName string
param location string
param tags object
param deployments array

resource cognitiveServicesAccount 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
	name: cognitiveServicesAccountName
	location: location
	tags: tags
	sku: {
		name: 'S0'
	}
	kind: 'CognitiveServices'
	properties: {
		apiProperties: {
			statisticsEnabled: false
		}
	}
}

@batchSize(1)
resource deployment 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = [for deployment in deployments: {
  parent: cognitiveServicesAccount
  name: deployment.name
  properties: {
    model: deployment.model
    raiPolicyName: contains(deployment, 'raiPolicyName') ? deployment.raiPolicyName : null
  }
  sku: contains(deployment, 'sku') ? deployment.sku : {
    name: 'Standard'
    capacity: 20
  }
}]

