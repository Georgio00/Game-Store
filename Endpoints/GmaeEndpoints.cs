using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;


namespace GameStore.Api.Endpoints;

    public static class GmaeEndPoints
    {

    const string GetGameEndpoint = "GetGame";


    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {

        var group = app.MapGroup("games");
        




        //GEt /games 
        group.MapGet("/",async (GameStoreContext dbcontext) =>
               await dbcontext.Games
                     .Include(game=>game.Genre)
                     .Select(game => game.ToGameSummaryDto())
                     .AsNoTracking()
                     .ToListAsync());

        //GET /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbcontext) =>
        {
            Game? game =await dbcontext.Games.FindAsync(id);

            return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        })
        .WithName(GetGameEndpoint);

        //POST
        group.MapPost("/", async (CreateGameDto newGame,GameStoreContext dbcontext ) =>
        {
            Game game = newGame.ToEntity();
            

            dbcontext.Games.Add(game);
           await dbcontext.SaveChangesAsync();

            GameSummaryDto gameDto = new(

                game.Id,
                game.Name,
                game.Genre!.Name,
                game.price,
                game.ReleaseDate

            );

            return Results.CreatedAtRoute(
                GetGameEndpoint, new { id = game.Id },
                game.ToGameDetailsDto());
        })
            .WithParameterValidation();

        //PUT /game  update game
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame= await dbContext.Games.FindAsync(id);  
            if (existingGame is null)
            {
                return Results.NotFound();
            }

         dbContext.Entry(existingGame).CurrentValues
                                      .SetValues(updatedGame.ToEntity(id)); 
           
           await dbContext.SaveChangesAsync();
               
            return Results.NoContent();
        });

        //DELETE /games/1
        group.MapDelete("/{id}",async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games
            .Where(game => game.Id == id)
            .ExecuteDeleteAsync();
                
          

            return Results.NoContent();
        });

        return group;

    }


}

