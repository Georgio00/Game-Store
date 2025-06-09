using GameStore.Api.Dtos;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GameStore.Api.Endpoints;

    public static class GmaeEndPoints
    {

    const string GetGameEndpoint = "GetGame";

      private static readonly List<GameDto> games = [

        new(
        1,
        "Street Fighting II",
        "Fighting",
        19.99M,
        new DateOnly(1992,7,15)),

    new(
        2,
        "Final Fantasy XIV",
        "Roleplaying",
        59.99M,
        new DateOnly(2010,9,30)),

     new(
        3,
        "FIFA",
        "Sport",
        69.99M,
        new DateOnly(2022,9,27)),


    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {

        var group = app.MapGroup("games");
            //.WithParameterValidation();
        



        //GEt /games 
        group.MapGet("/", () => games);

        //GET /games/1
        group.MapGet("/{id}", (int id) =>
        {
            GameDto? game = games.Find(game => game.Id == id);

            return game is null ? Results.NotFound() : Results.Ok(game);
        })
        .WithName(GetGameEndpoint);

        //POST
        group.MapPost("/", (CreateGameDto newGame) =>
        {


            GameDto game = new(
                games.Count + 1,
                newGame.Name,
                newGame.Genre,
                newGame.Price,
                newGame.ReleaseDate
            );

            games.Add(game);

            return Results.CreatedAtRoute("GetGame", new { id = game.Id }, game);
        })
            .WithParameterValidation();

        //PUT /game  update game
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

        //DELETE /games/1
        group.MapDelete("/{id}", (int id) =>
        {
            games.RemoveAll(game => game.Id == id);

            return Results.NoContent();
        });

        return group;

    }


}

