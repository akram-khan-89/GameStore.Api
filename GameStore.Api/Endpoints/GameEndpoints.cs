using System;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Endpoints;

public static class GameEndpoints
{
    const string GetGameEndpointName = "GetGame";

    private static readonly List<GameDto> games = [
    new (
    1,
    "Street Fighter 2",
    "Fighting",
    19.99M,
    new DateOnly(1992, 7, 15)),

    new (
    2,
    "Final Fantasy 5",
    "Roleplaying",
    59.99M,
    new DateOnly(2010, 9, 30)),

    new (
    3,
    "FIFA",
    "Sports",
    79.99M,
    new DateOnly(2020, 4, 10)),

];

    public static RouteGroupBuilder MapGameEndpoints(this WebApplication app)
    {

        var group = app.MapGroup("games").WithParameterValidation();
        // GET /games
        group.MapGet("/", () => games);

        // GET /games/1
        group.MapGet("/{id}", (int id) =>
        {
            GameDto? game = games.Find(game => game.Id == id);

            return game is null ? Results.NotFound() : Results.Ok(game);
        })
        .WithName(GetGameEndpointName);


        // POST /games
        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = new()
            {
                Name = newGame.Name,
                Genre = dbContext.Genres.Find(newGame.GenreId),
                GenreId = newGame.GenreId,
                Price = newGame.Price,
                ReleaseDate = newGame.ReleaseDate

            };
            // GameDto game = new(
            //     games.Count + 1,
            //     newGame.Name,
            //     newGame.Genre,
            //     newGame.Price,
            //     newGame.ReleaseDate
            // );

            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            GameDto gameDto = new
            (
                game.Id,
                game.Name,
                game.Genre!.Name,
                game.Price,
                game.ReleaseDate

        );

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, gameDto);
        });

        // PUT /games/1
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
        {
            var index = games.FindIndex(game => game.Id == id);

            if (index == -1)
            {
                return Results.NotFound();
            }

            games[index] = new GameDto(
                id,
                updatedGame.Name,
                updatedGame.Genre,
                updatedGame.Price,
                updatedGame.ReleaseDate
            );

            return Results.NoContent();
        });

        // DELETE /games/1
        group.MapDelete("/{id}", (int id) =>
        {
            games.RemoveAll(game => game.Id == id);
            return Results.NoContent();
        });

        return group;
    }

}
