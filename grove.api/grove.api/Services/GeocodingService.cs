using Geocoding;
using Geocoding.Microsoft;
using grove.Configuration;
using Microsoft.Extensions.Options;

namespace grove.Services;

public interface IGeocodingService
{
    Task<Location> GetCoordinates(string address);
}

public class GeocodingService : IGeocodingService
{
    private string geokey { get; set; }
    
    public GeocodingService(IOptions<APISettings> options)
    {
        geokey = options.Value.ConnectionString;
    }
    
    public async Task<Location> GetCoordinates(string address)
    {
        var geocoding = geokey;
        IGeocoder geocoder = new BingMapsGeocoder(geocoding);
        IEnumerable<Address> addresses = await geocoder.GeocodeAsync(address);
        var location = addresses.First().Coordinates;
        return location;
    }
}

public class GeocodingRandomService : IGeocodingService
{
    // Random Location
    public async Task<Location> GetCoordinates(string address)
    {
        var random = new Random();
        var location = new Location(random.NextDouble() * 180 - 90, random.NextDouble() * 360 - 180);
        return location;
    }
}