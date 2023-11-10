using Microsoft.Extensions.Logging.ApplicationInsights;
using RecommendationApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
               builder =>
               {
                   builder.WithOrigins(new string[] { "http://localhost:3000", config["Cors:AllowedOrigins"] }).WithHeaders("content-type");
               });
});

builder.Services.AddDaprClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAzureServices();
builder.Services.AddHealthChecks();

string connectionString = config["ApplicationInsights:ConnectionString"];

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddFilter<ApplicationInsightsLoggerProvider>(logLevel => logLevel == LogLevel.Information);
    loggingBuilder.SetMinimumLevel(LogLevel.Information);
});

builder.Services.AddApplicationInsightsTelemetryWorkerService(options =>
{
    options.ConnectionString = connectionString;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.MapApi();
app.MapHealthChecks("/healthz");

app.Run();
