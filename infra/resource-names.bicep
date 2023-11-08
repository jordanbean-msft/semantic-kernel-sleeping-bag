param resourceToken string

var abbrs = loadJsonContent('./abbreviations.json')

output appInsightsName string = '${abbrs.insightsComponents}${resourceToken}'
output logAnalyticsWorkspaceName string = '${abbrs.operationalInsightsWorkspaces}${resourceToken}'
output cognitiveServicesAccountName string = '${abbrs.cognitiveServicesAccounts}${resourceToken}'
output cognitiveServicesAcccountOpenAiName string = '${abbrs.cognitiveServicesAccounts}openai-${resourceToken}'
output containerAppEnvironmentName string = '${abbrs.appManagedEnvironments}${resourceToken}'
output containerRegistryName string = '${abbrs.containerRegistryRegistries}${resourceToken}'
