namespace grove.Database;

using Dapper;

public class DbInitializer
{
    private IDbConnectionFactory _dbConnectionFactory;

    public DbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        // Create the Event table if it doesn't exist
        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS Event (
                Id UNIQUEIDENTIFIER PRIMARY KEY, 
                Name NVARCHAR(255),
                Description NVARCHAR(MAX),
                X FLOAT,
                Y FLOAT,
                Date DATETIME
            )");

        // Create the User table if it doesn't exist
        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS [User] (
                Id UNIQUEIDENTIFIER PRIMARY KEY, 
                Name NVARCHAR(255),
                X FLOAT,
                Y FLOAT
            )");

    }
}