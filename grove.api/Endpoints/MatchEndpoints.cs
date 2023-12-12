using grove.Repository;
using Microsoft.AspNetCore.Mvc;

namespace grove.Endpoints;

public static class MatchEndpoints
{
    public static void RegisterMatchEndpoints(this WebApplication app)
    {
        var matches = app.MapGroup("/matches")
            .WithTags("matches");
        matches.MapPost("/{id}",CreateMatch);
        matches.MapGet("/{id}",GetMatches);
        matches.MapDelete("/{userId}/{eventId}",DeleteMatch);
        matches.MapGet("/{userId}/{eventId}", GetMatch);
    }
    
    static async Task<IResult> CreateMatch([FromQuery]Guid id, [FromServices]UserDb userDb, [FromBody]Event evnt)
    {
        var user = await userDb.Users.FindAsync(id);
        user?.matchedEventIds.Add(evnt.id);
        await userDb.SaveChangesAsync();
        return Results.NoContent();
    }

    static async Task<IResult> GetMatches([FromQuery]Guid id, UserDb userDb)
    {
        var user = await userDb.Users.FindAsync(id);
        return TypedResults.Ok(user?.matchedEventIds);
    }

    static async Task<IResult> DeleteMatch([FromQuery]Guid userId, [FromServices]UserDb userDb, [FromQuery]Guid eventId)
    {
        var user = await userDb.Users.FindAsync(userId);
        if (user is null) return Results.NotFound();
        if (user.matchedEventIds.Contains(eventId))
        {
            user.matchedEventIds.Remove(eventId);   
            await userDb.SaveChangesAsync();
            return Results.NoContent();
        }
        return Results.NotFound();
    }   

    static async Task<IResult> GetMatch([FromQuery]Guid userId, [FromServices]UserDb userDb, [FromQuery]Guid eventId)
    {
        var user = await userDb.Users.FindAsync(userId);
        if (user is null) return Results.NotFound();
        if (user.matchedEventIds.Contains(eventId))
        {
            return TypedResults.Ok(eventId);
        }
        return Results.NotFound();
    }
}