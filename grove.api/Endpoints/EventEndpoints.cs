using grove.DTOModels;
using grove.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace grove.Endpoints;

public static class EventEndpoints
{
    public static void RegisterEventEndpoints(this WebApplication app)
    {
        var events = app.MapGroup("/events")
            .WithTags("events");
        events.MapGet("/",GetEvents);
        events.MapGet("/{id}",GetEvent);
        events.MapPost("/",CreateEvent);
        events.MapPut("/",UpdateEvent);
        events.MapDelete("/",DeleteEvent);
        events.MapGet("/radius/{id}",GetEventsInRadius);
        events.MapPost("/user/{id}",CreateUserEvent);
        events.MapGet("/user/{id}",GetUserEventList);
    }
    
    static async Task<IResult> CreateEvent([FromBody]EventDTO eventDto, [FromServices]EventDb db)
    {
        var evnt = new Event();
        evnt.name = eventDto.name;
        evnt.description = eventDto.description;
        evnt.X = eventDto.X;
        evnt.Y = eventDto.Y;
        evnt.date = eventDto.date;
        evnt.id = Guid.NewGuid();
        db.Add(evnt);
        await db.SaveChangesAsync();
        var evntDTO = new EventDTO(evnt);
        return TypedResults.Ok(evntDTO);
    }

    static async Task<IResult> GetEvents([FromServices]EventDb db)
    {
        return TypedResults.Ok(await db.Events.ToListAsync());
    }

    static async Task<IResult> GetEvent([FromQuery]Guid id, [FromServices]EventDb db)
    {
        return Results.Ok(await db.FindAsync<Event>(id));
    }

    static async Task<IResult> CreateUserEvent([FromBody]EventDTO eventDto, [FromServices]EventDb db, [FromQuery]Guid id, [FromServices]UserDb userDb)
    {
        var evnt = new Event();
        evnt.name = eventDto.name;
        evnt.description = eventDto.description;
        evnt.X = eventDto.X;
        evnt.Y = eventDto.Y;
        evnt.date = eventDto.date;
        evnt.id = Guid.NewGuid();
        db.Add(evnt);
        await db.SaveChangesAsync();

        var user = await userDb.Users.FindAsync(id);
        user?.createdEventIds.Add(evnt.id);
        await userDb.SaveChangesAsync();

        var evntDTO = new EventDTO(evnt);
        return TypedResults.Ok(evntDTO);
    }

    static async Task<IResult> GetUserEventList([FromQuery]Guid id, [FromServices]UserDb udb, [FromServices]EventDb db)
    {
        var _user = await udb.FindAsync<User>(id);
        var events = _user.createdEventIds
            .Select(e => db.FindAsync<Event>(e).Result)
            .Where(e => e != null);
        return TypedResults.Ok(events);
    }

    // GET events in radius
    // distance query
    // haversine from events
    // return events
    static async Task<IResult> GetEventsInRadius([FromQuery]Guid id, [FromServices]EventDb db, [FromServices]UserDb userDb)
    {
        var user = await userDb.Users.FindAsync(id);
        if (user is null)
        {
            return Results.NotFound();
        }

        var events = await db.Events.ToListAsync();
        var evnt = Haversine.GetNearestNeighbor(user, events);
        return TypedResults.Ok(evnt);
    }

    static async Task<IResult> UpdateEvent([FromBody]EventDTO evntDto, [FromServices]EventDb db)
    {
        var evnt = await db.Events.FindAsync(evntDto.id);
        if (evnt is null) return Results.NotFound();
        
        evnt.name = evntDto.name;
        evnt.description = evntDto.description;
        evnt.X = evntDto.X;
        evnt.Y = evntDto.Y;
        
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    static async Task<IResult> DeleteEvent([FromQuery]Guid id, [FromServices]EventDb db)
    {
        var deleteEvent = await db.Events.FindAsync(id);
        if (deleteEvent is null) return Results.NotFound();
        
        db.Events.Remove(deleteEvent);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}