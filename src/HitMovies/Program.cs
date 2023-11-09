var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

List<HitMovie> hits = new()
{
    new(){
        Title = "GalaxyQuest",
        Actors = new(){ "Rainn Wilson" },
        Tags = new(){ "comedy", "sci-fi" }
    }
};

app.MapGet("/hitMoviesByTag", (List<string> tags) =>
{
    return TypedResults.Ok(hits.Find(x => x.Tags.Intersect(tags).Any()));
})
.WithName("GetHitMoviesByTag")
.WithOpenApi();

app.MapHealthChecks("/healthz");

app.Run();

