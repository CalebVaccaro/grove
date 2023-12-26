using grove.DTOModels;
using grove.Repository;
using grove.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace grove.Endpoints;

public static class UserEndpoints
{
    public static void RegisterUserEndpoints(this WebApplication app)
    {
        var users = app.MapGroup("/users")
            .WithTags("users");
        users.MapPost("/", CreateUser);
        users.MapGet("/", GetUsers);
        users.MapGet("/{id}", GetUser);
        users.MapPut("/{id}", UpdateUser);
        users.MapDelete("/{id}", DeleteUser);
    }

    static async Task<IResult> CreateUser([FromBody]UserDTO userDto, [FromServices]UserDb db, [FromServices] IGeocodingService geocodingService)
    {
        var user = new User();
        user.id = Guid.NewGuid();
        user.name = userDto.name;
        user.createdEventIds = userDto.createdEventIds;
        user.matchedEventIds = userDto.matchedEventIds;
        
        user.address = userDto.address;
        var location = await geocodingService.GetCoordinates(userDto.address);
        user.X = location.Latitude;
        user.Y = location.Longitude;
        
        db.Add(user);
        await db.SaveChangesAsync();

        var userDTO = new UserDTO(user);
        return TypedResults.Ok(userDTO);
    }

    // Get
    static async Task<IResult> GetUsers([FromServices]UserDb db)
    {
        return TypedResults.Ok(await db.Users.ToListAsync());
    }

    static async Task<IResult> GetUser(Guid id, [FromServices]UserDb db)
    {
        return TypedResults.Ok(await db.FindAsync<User>(id));
    }

    // Put
    static async Task<IResult> UpdateUser(Guid id, [FromBody]UserDTO userDTO, [FromServices]UserDb db, [FromServices] IGeocodingService geocodingService)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return Results.NotFound();
        
        user.name = userDTO.name;
        user.createdEventIds = userDTO.createdEventIds;
        user.matchedEventIds = userDTO.matchedEventIds;

        if (user.address != userDTO.address)
        {
            var location = await geocodingService.GetCoordinates(userDTO.address);
            user.X = location.Latitude;
            user.Y = location.Longitude;
        }
        
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    // Delete
    static async Task<IResult> DeleteUser(Guid id , [FromServices]UserDb db)
    {
        var deleteUser = await db.Users.FindAsync(id);
        if (deleteUser is null) return Results.NotFound();
        
        db.Users.Remove(deleteUser);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}