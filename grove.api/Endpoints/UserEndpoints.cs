using grove.DTOModels;
using grove.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace grove.Endpoints;

public static class UserEndpoints
{
    public static void RegisterUserEndpoints(this WebApplication app)
    {
        var users = app.MapGroup("/users")
            .WithTags("users")
            .RequireAuthorization();
        users.MapPost("/", CreateUser);
        users.MapGet("/", GetUsers);
        users.MapGet("/{id}", GetUser);
        users.MapPut("/", UpdateUser);
        users.MapDelete("/", DeleteUser);
    }

    static async Task<IResult> CreateUser([FromBody]UserDTO userDto, [FromServices]UserDb db)
    {
        var user = new User();
        user.id = Guid.NewGuid();
        user.name = userDto.name;
        user.createdEventIds = userDto.createdEventIds;
        user.matchedEventIds = userDto.matchedEventIds;
        user.X = userDto.X;
        user.Y = userDto.Y;
        
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

    static async Task<IResult> GetUser([FromQuery]Guid id, [FromServices]UserDb db)
    {
        return TypedResults.Ok(await db.FindAsync<User>(id));
    }

    // Put
    static async Task<IResult> UpdateUser([FromBody]UserDTO userDTO, [FromServices]UserDb db)
    {
        var user = await db.Users.FindAsync(userDTO.id);
        if (user is null) return Results.NotFound();
        
        user.name = userDTO.name;
        user.createdEventIds = userDTO.createdEventIds;
        user.matchedEventIds = userDTO.matchedEventIds;
        user.X = userDTO.X;
        user.Y = userDTO.Y;
        
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    // Delete
    static async Task<IResult> DeleteUser([FromBody]Guid id , [FromServices]UserDb db)
    {
        var deleteUser = await db.Users.FindAsync(id);
        if (deleteUser is null) return Results.NotFound();
        
        db.Users.Remove(deleteUser);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}