using HitTVShows;
using Microsoft.Extensions.FileProviders;

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

List<HitTVShow> hits =
[
    new(){
        Title = "The Office",
        Tags = ["comedy", "documentary"],
        Actors = ["Steve Carell", "Jenna Fischer", "John Krasinski", "Rainn Wilson"],
        SeasonAiredYears = [2005, 2006, 2007, 2008, 2009, 2010, 2011, 2012, 2013]
    }
];

app.MapGet("/hitTVShowsByYear", (int year) =>
{
    return TypedResults.Ok(hits.Find(x => x.SeasonAiredYears.Contains(year)));
})
.WithName("GetHitTVShows")
.WithOpenApi();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(builder.Environment.ContentRootPath),
    RequestPath = "/.well-known"
});


app.MapHealthChecks("/healthz");

app.Run();
