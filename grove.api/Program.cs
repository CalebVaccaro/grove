using grove.Endpoints;
using grove.Repository;
using grove.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<EventDb>(options => options.UseSqlite("Data Source=grove.db"));
builder.Services.AddDbContext<UserDb>(options => options.UseSqlite("Data Source=grove.db"));

// add geocoding service as a singleton
builder.Services.AddSingleton<IGeocodingService, GeocodingService>();
builder.Services.AddSingleton<IBlobStorageService, MockBlobStorageService>();

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


