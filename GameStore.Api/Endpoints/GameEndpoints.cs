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
        group.MapGet("/", async (GameStoreContext dbContext) =>

          // display game with their genres 
          // (also with some optimization using AsNoTracking() )
          await dbContext.Games
            .Include(game => game.Genre)
            .Select(game => game.ToGameSummaryDto())
            .AsNoTracking()
            .ToListAsync());

        // GET /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(id);

            return game is null ?
                   Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        })
        .WithName(GetGameEndpointName);


        // POST /games
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            // Mapping entities to DTOs
            Game game = newGame.ToEntity();

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(GetGameEndpointName,
             new { id = game.Id },
              game.ToGameDetailsDto());
        });

        // PUT /games/1
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGames = await dbContext.Games.FindAsync(id);

            if (existingGames is null)
            {
                return Results.NotFound();
            }

            dbContext.Entry(existingGames)
            .CurrentValues
            .SetValues(updatedGame.ToEntity(id));

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE /games/1
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            // deleting game from data also called (batch delete)
            await dbContext.Games
             .Where(game => game.Id == id)
             .ExecuteDeleteAsync();

            return Results.NoContent();
        });

        return group;
    }

}
