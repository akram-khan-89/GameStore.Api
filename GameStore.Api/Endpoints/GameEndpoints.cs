using System;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GameEndpoints
{
    const string GetGameEndpointName = "GetGame";

    public static RouteGroupBuilder MapGameEndpoints(this WebApplication app)
    {

        var group = app.MapGroup("games").WithParameterValidation();
        // GET /games
        group.MapGet("/", (GameStoreContext dbContext) =>

            // display game with their genres 
            // (also with some optimization using AsNoTracking())
            dbContext.Games
            .Include(game => game.Genre)
            .Select(game => game.ToGameSummaryDto())
            .AsNoTracking());

        // GET /games/1
        group.MapGet("/{id}", (int id, GameStoreContext dbContext) =>
        {
            Game? game = dbContext.Games.Find(id);

            return game is null ?
                   Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        })
        .WithName(GetGameEndpointName);


        // POST /games
        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            // Mapping entities to DTOs
            Game game = newGame.ToEntity();

            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute(GetGameEndpointName,
             new { id = game.Id },
              game.ToGameDetailsDto());
        });

        // PUT /games/1
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGames = dbContext.Games.Find(id);

            if (existingGames is null)
            {
                return Results.NotFound();
            }

            dbContext.Entry(existingGames)
            .CurrentValues
            .SetValues(updatedGame.ToEntity(id));

            dbContext.SaveChanges();

            return Results.NoContent();
        });

        // DELETE /games/1
        group.MapDelete("/{id}", (int id, GameStoreContext dbContext) =>
        {
            // deleting game from data also called (batch delete)
            dbContext.Games
            .Where(game => game.Id == id)
            .ExecuteDelete();

            return Results.NoContent();
        });

        return group;
    }

}
