var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

List<HistoricalSportsScore> sportsResults = new()
{
    new HistoricalSportsScore{
        Team1 = "Texas Tech",
        Team2 = "Minnesota",
        Team1Score = 44,
        Team2Score = 41,
        GameDate = DateTime.Parse("Dec 29, 2006"),
        Sport = "Football",
        TeamThatWon = "Texas Tech"
    }
};

app.MapGet("/historicalSportsScores", () =>
{
    return TypedResults.Ok(sportsResults);
})
.WithName("GetHistoricalSportsScores");

app.Run();
