using grove.Configuration;
using grove.Endpoints;
using grove.Repository;
using grove.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<APISettings>(builder.Configuration.GetSection("APISettings"));

// get connection string from IOpetions<APISettings>
var connectionString = builder.Configuration.GetSection("APISettings")["DBConnectionString"];

builder.Services.AddDbContext<EventDb>(options => options.UseSqlite(connectionString));
builder.Services.AddDbContext<UserDb>(options => options.UseSqlite(connectionString));

// add geocoding service as a singleton
builder.Services.AddSingleton<IGeocodingService, GeocodingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.RegisterUserEndpoints();
app.RegisterEventEndpoints();
app.RegisterMatchEndpoints();


app.Run();


