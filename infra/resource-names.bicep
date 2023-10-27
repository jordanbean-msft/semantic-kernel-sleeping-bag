param resourceToken string

var abbrs = loadJsonContent('./abbreviations.json')

output appInsightsName string = '${abbrs.insightsComponents}${resourceToken}'
output logAnalyticsWorkspaceName string = '${abbrs.operationalInsightsWorkspaces}${resourceToken}'
