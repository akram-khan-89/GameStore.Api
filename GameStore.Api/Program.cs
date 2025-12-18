using GameStore.Api.Data;
using GameStore.Api.Endpoints;
var builder = WebApplication.CreateBuilder(args);

var connString = "Data Source=GameStore.db";

// Register services
builder.Services.AddSqlite<GameStoreContext>(connString);

var app = builder.Build();

app.MapGameEndpoints();

app.Run();
