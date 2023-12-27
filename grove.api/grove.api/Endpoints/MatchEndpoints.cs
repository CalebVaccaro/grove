using grove.DTOModels;
using grove.Repository;
using Microsoft.AspNetCore.Mvc;

namespace grove.Endpoints;

public static class MatchEndpoints
{
    public static void RegisterMatchEndpoints(this WebApplication app)
    {
        var matches = app.MapGroup("/matches")
            .WithTags("matches");
        matches.MapPost("/{userId}",CreateMatch);
        matches.MapGet("/{userId}",GetMatches);
        matches.MapDelete("/{userId}/{eventId}",DeleteMatch);
        matches.MapGet("/{userId}/{eventId}", GetMatch);
    }
    
    static async Task<IResult> CreateMatch(Guid userId, [FromServices]UserDb userDb, [FromBody]EventDTO evnt)
    {
        var user = await userDb.Users.FindAsync(userId);
        if (!user.matchedEventIds.Contains(evnt.id))
        {
            user.matchedEventIds.Add(evnt.id);
            user.lastPartition = evnt.date;
        }
        await userDb.SaveChangesAsync();
        return Results.NoContent();
    }

    static async Task<IResult> GetMatches(Guid userId, [FromServices]UserDb userDb)
    {
        var user = await userDb.Users.FindAsync(userId);
        return TypedResults.Ok(user?.matchedEventIds);
    }

    static async Task<IResult> DeleteMatch(Guid userId, [FromServices]UserDb userDb, Guid eventId)
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

    static async Task<IResult> GetMatch(Guid userId, [FromServices]UserDb userDb, Guid eventId)
    {
        var user = await userDb.Users.FindAsync(userId);
        if (user is null) return Results.NotFound();
        return user.matchedEventIds.Contains(eventId) ? TypedResults.Ok(eventId) : Results.NotFound();
    }
}