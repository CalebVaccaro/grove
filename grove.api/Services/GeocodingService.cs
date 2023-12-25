using Geocoding;
using Geocoding.Microsoft;

namespace grove.Services;

public interface IGeocodingService
{
    Task<Location> GetCoordinates(string address);
}

public class GeocodingService : IGeocodingService
{
    public async Task<Location> GetCoordinates(string address)
    {
        var geocoding = "Apr_ExyNxxPkppFgkJq6ZHJdpB4uOBMJzqr-GWdcLylHwSD9g_n3hr4W6hNds9xc";
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