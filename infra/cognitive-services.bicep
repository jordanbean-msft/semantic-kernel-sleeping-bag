param cognitiveServicesAccountName string
param cognitiveServicesAccountOpenAiName string
param location string
param tags object

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

resource cognitiveServicesAccountOpenAi 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
	name: cognitiveServicesAccountOpenAiName
	location: location
	tags: tags
	sku: {
		name: 'S0'
	}
	kind: 'OpenAI'
	properties: {
		apiProperties: {
			statisticsEnabled: false
		}
	}
}