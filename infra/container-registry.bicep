param containerRegistryName string
param location string
param tags object

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
	name: containerRegistryName
	location: location
	tags: tags
	sku: {
		name: 'Standard'
	}
	properties: {
		adminUserEnabled: true
	}
}

output containerRegistryName string = containerRegistry.name