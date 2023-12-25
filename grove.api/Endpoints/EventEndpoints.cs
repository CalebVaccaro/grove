using Geocoding;
using grove.DTOModels;
using grove.Repository;
using grove.Services;
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
    
    static async Task<IResult> CreateEvent([FromBody]EventDTO eventDto, [FromServices]EventDb db, [FromServices]IGeocodingService geocodingService)
    {
        var evnt = new Event();
        evnt.name = eventDto.name;
        evnt.description = eventDto.description;

        if (eventDto.address == null || eventDto.address == "" || eventDto.address == "string")
        {
            return TypedResults.BadRequest("Address cannot be empty");
        }

        // Get Location Coordinates
        Location location;
        try
        {
            location = await geocodingService.GetCoordinates(eventDto.address);
        }
        catch (Exception e)
        {
            return TypedResults.BadRequest("Address cannot be parsed");
        }

        evnt.X = location.Latitude;
        evnt.Y = location.Longitude;
        evnt.address = eventDto.address;
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

    static async Task<IResult> CreateUserEvent([FromBody]EventDTO eventDto, 
        [FromServices]EventDb db, [FromQuery]Guid id, [FromServices]UserDb userDb,
        [FromServices]IGeocodingService geocodingService)
    {
        var evnt = new Event();
        evnt.name = eventDto.name;
        evnt.description = eventDto.description;
        
        // Get Location Coordinates
        var location = await geocodingService.GetCoordinates(eventDto.address);
        evnt.X = location.Latitude;
        evnt.Y = location.Longitude;
        
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
        var evnt = HaversineService.GetNearestNeighbor(user, events, 500);
        return TypedResults.Ok(evnt);
    }

    static async Task<IResult> UpdateEvent([FromQuery] Guid id, [FromBody]EventDTO eventDto, [FromServices]EventDb db, [FromServices]IGeocodingService geocodingService)
    {
        var evnt = await db.Events.FindAsync(id);
        if (evnt is null) return Results.NotFound();
        
        evnt.name = eventDto.name;
        
        // Get Location Coordinates
        if (eventDto.address != evnt.address)
        {
            evnt.address = eventDto.address;
            var location = await geocodingService.GetCoordinates(eventDto.address);
            evnt.X = location.Latitude;
            evnt.Y = location.Longitude;
        }
        
        evnt.description = eventDto.description;
        evnt.date = eventDto.date;
        evnt.Image = eventDto.image;
        
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