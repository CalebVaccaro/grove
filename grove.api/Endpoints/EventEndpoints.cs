using Geocoding;
using grove.DTOModels;
using grove.Repository;
using grove.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
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
        events.MapGet("/radius/{userId}",GetEventsInRadius);
        events.MapGet("/user/{userId}",GetUserEventList);
        events.MapGet("/{userId}/{eventId}", AddEventToUser);
    }
    
    static async Task<IResult> CreateEvent([FromBody]EventDTO eventDto,
        [FromServices]EventDb db,
        [FromServices]IGeocodingService geocodingService,
        [FromServices]IBlobStorageService blobStorageService)
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
            evnt.X = location.Latitude;
            evnt.Y = location.Longitude;
        }
        catch (Exception e)
        {
            return TypedResults.BadRequest("Address cannot be parsed");
        }

        evnt.address = eventDto.address;
        evnt.date = eventDto.date;
        evnt.id = Guid.NewGuid();
        
        // Upload Image to Blob Storage
        if (eventDto.image != null)
        {
            var blobName = $"{evnt.id}.jpg";
            await blobStorageService.UploadBlobAsync(blobName);
            evnt.Image = blobName;
        }
        
        db.Add(evnt);
        await db.SaveChangesAsync();
        
        var evntDTO = new EventDTO(evnt);
        return TypedResults.Ok(evntDTO);
    }

    static async Task<IResult> GetEvents([FromServices]EventDb db)
    {
        return TypedResults.Ok(await db.Events.ToListAsync());
    }

    static async Task<IResult> GetEvent(Guid id, [FromServices]EventDb db)
    {
        return Results.Ok(await db.FindAsync<Event>(id));
    }

    static async Task<IResult> UpdateEvent(Guid id, [FromBody]EventDTO eventDto, [FromServices]EventDb db, [FromServices]IGeocodingService geocodingService)
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

    static async Task<IResult> DeleteEvent(Guid id, [FromServices]EventDb db)
    {
        var deleteEvent = await db.Events.FindAsync(id);
        if (deleteEvent is null) return Results.NotFound();
        
        db.Events.Remove(deleteEvent);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    
    static async Task<IResult> AddEventToUser(Guid userId, [FromServices] UserDb userDb, Guid eventId,
        [FromServices] EventDb eventDb)
    {
        var user = await userDb.Users.FindAsync(userId);
        if (user is null) return Results.NotFound();
        var evnt = await eventDb.Events.FindAsync(eventId);
        if (evnt is null) return Results.NotFound();
        user.createdEventIds.Add(evnt.id);
        await userDb.SaveChangesAsync();
        return Results.NoContent();
    }
    
    static async Task<IResult> GetUserEventList(Guid userId, [FromServices]UserDb udb, [FromServices]EventDb db)
    {
        var _user = await udb.FindAsync<User>(userId);
        var events = _user.createdEventIds
            .Select(e => db.FindAsync<Event>(e).Result)
            .Where(e => e != null);
        return TypedResults.Ok(events);
    }

    // GET events in radius
    // distance query
    // haversine from events
    // return events
    static async Task<IResult> GetEventsInRadius(Guid userId, [FromQuery] double distance, [FromServices]EventDb db, [FromServices]UserDb userDb)
    {
        var user = await userDb.Users.FindAsync(userId);
        if (user is null)
        {
            return Results.NotFound();
        }

        var events = await db.Events.ToListAsync();
        var eventsInDistance = HaversineService.GetNearestNeighbor(user, events, distance);
        return TypedResults.Ok(eventsInDistance);
    }
}