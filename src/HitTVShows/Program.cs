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

List<HitTVShow> hits = new()
{
    new(){
        Title = "The Office",
        Tags = new(){ "comedy", "documentary" },
        Actors = new(){ "Steve Carell", "Jenna Fischer", "John Krasinski", "Rainn Wilson" },
        SeasonAiredYears = new(){ 2005, 2006, 2007, 2008, 2009, 2010, 2011, 2012, 2013 }
    }
};

app.MapGet("/hitTVShowsByYear", (int year) =>
{
    return TypedResults.Ok(hits.Find(x => x.SeasonAiredYears.Contains(year)));
})
.WithName("GetHitTVShows")
.WithOpenApi();

app.MapHealthChecks("/healthz");

app.Run();
