var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
               builder =>
               {
                   builder.WithOrigins("http://localhost:3000").WithHeaders("content-type");
               });
});

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
app.UseCors("AllowAll");

app.MapPost("/recommendation", (Request request) =>
{
    return TypedResults.Ok(new Response
    {
        Message = $"You should do this: {request.Message}"
    });
})
.WithName("GetRecommendation")
.WithOpenApi();

app.Run();
