var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddSingleton<SimWeb.Services.GameService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// API endpoint for auto-play
app.MapPost("/api/nextTurn", (SimWeb.Services.GameService gameService) =>
{
    gameService.EnsureInitialized();
    var success = gameService.NextTurn();
    return Results.Json(gameService.GetMapState());
});

app.MapGet("/api/state", (SimWeb.Services.GameService gameService) =>
{
    gameService.EnsureInitialized();
    return Results.Json(gameService.GetMapState());
});

app.Run();
