using Aspire.Dashboard.Components.Pages;

var builder = DistributedApplication.CreateBuilder(args);

var appInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];

builder.AddDapr();

var historicalWeatherLookup = builder.AddProject<Projects.HistoricalWeatherLookup>("historical-weather-lookup")
    .WithEnvironment("ApplicationInsights:ConnectionString", appInsightsConnectionString)
    .WithDaprSidecar();

var locationLookup = builder.AddProject<Projects.LocationLookup>("location-lookup")
    .WithEnvironment("ApplicationInsights:ConnectionString", appInsightsConnectionString)
    .WithDaprSidecar();

var orderHistory = builder.AddProject<Projects.OrderHistory>("order-history")
    .WithEnvironment("ApplicationInsights:ConnectionString", appInsightsConnectionString)
    .WithDaprSidecar();

var productCatalog = builder.AddProject<Projects.ProductCatalog>("product-catalog")
    .WithEnvironment("ApplicationInsights:ConnectionString", appInsightsConnectionString)
    .WithDaprSidecar();

var recommendationApi = builder.AddProject<Projects.RecommendationApi>("recommendation-api")
    .WithEnvironment("ApplicationInsights:ConnectionString", appInsightsConnectionString)
    .WithReference(historicalWeatherLookup)
    .WithReference(locationLookup)
    .WithReference(orderHistory)
    .WithReference(productCatalog)
    .WithDaprSidecar();

//var recommendationWebApp = builder.AddNpmApp("recommendation-web-app", "../recommendation-web-app", "start")
//    .WithReference(recommendationApi)
//    .WithServiceBinding(hostPort: 3000, scheme: "http");

builder.Build().Run();
