using Alligator.ConnectFour.Web;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5000");
builder.Services.AddSingleton<GameService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/state", (GameService game) => game.GetState());

app.MapPost("/api/new", (GameService game) => game.NewGame());

app.MapPost("/api/move", (MoveRequest request, GameService game) =>
{
    try
    {
        return Results.Ok(game.MakeMove(request.Column));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();

record MoveRequest(int Column);
