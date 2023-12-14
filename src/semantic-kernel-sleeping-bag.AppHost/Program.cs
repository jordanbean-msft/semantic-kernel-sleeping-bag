using Aspire.Dashboard.Components.Pages;

var builder = DistributedApplication.CreateBuilder(args);

var appInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];

builder.AddDapr();

var historicalWeatherLookup = builder.AddProject<Projects.HistoricalWeatherLookup>("historicalWeatherLookup")
    .WithEnvironment("ApplicationInsights:ConnectionString", appInsightsConnectionString)
    .WithDaprSidecar("historical-weather-lookup")
    .WithServiceBinding(hostPort: 53558);

var locationLookup = builder.AddProject<Projects.LocationLookup>("locationLookup")
    .WithEnvironment("ApplicationInsights:ConnectionString", appInsightsConnectionString)
    .WithDaprSidecar("location-lookup")
    .WithServiceBinding(hostPort: 53560);

var orderHistory = builder.AddProject<Projects.OrderHistory>("orderHistory")
    .WithEnvironment("ApplicationInsights:ConnectionString", appInsightsConnectionString)
    .WithDaprSidecar("order-history")
    .WithServiceBinding(hostPort: 53562);

var productCatalog = builder.AddProject<Projects.ProductCatalog>("productCatalog")
    .WithEnvironment("ApplicationInsights:ConnectionString", appInsightsConnectionString)
    .WithDaprSidecar("product-catalog")
    .WithServiceBinding(hostPort: 53564);

var recommendationApi = builder.AddProject<Projects.RecommendationApi>("recommendationApi")
    .WithEnvironment("ApplicationInsights:ConnectionString", appInsightsConnectionString)
    .WithReference(historicalWeatherLookup)
    .WithReference(locationLookup)
    .WithReference(orderHistory)
    .WithReference(productCatalog)
    .WithDaprSidecar("recommendation-api")
    .WithServiceBinding(hostPort: 53566);

var recommendationWebApp = builder.AddNpmApp("recommendation-web-app", "../recommendation-web-app", "start")
    .WithReference(recommendationApi)
    .WithServiceBinding(scheme: "http");

builder.Build().Run();
