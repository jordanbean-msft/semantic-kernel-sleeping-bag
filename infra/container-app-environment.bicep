param containerAppEnvironmentName string
param location string
param tags object

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' = {
	name: containerAppEnvironmentName 
	location: location
	tags: tags
	properties: {
	}
}

output containerAppEnvironmentName string = containerAppEnvironment.name
