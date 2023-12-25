using System.ComponentModel;
using grove;

// HAVERSINE REFERENCE SCRIPT: https://gist.github.com/jammin77/033a332542aa24889452
/// <summary>
/// The distance type to return the results in.
/// </summary>
public static class HaversineService
{
    public enum DistanceType
    {
        Miles,
        Kilometers
    };

    /// <summary>
    /// Returns the distance in miles or kilometers of any two
    /// latitude / longitude points.
    /// </summary>
    /// <param name=”pos1″></param>
    /// <param name=”pos2″></param>
    /// <param name=”type”></param>
    /// <returns></returns>
    private static double Distance(User pos1, Event pos2, DistanceType type = DistanceType.Kilometers)
    {
        // Get Distance Ref Type
        double R = (type == DistanceType.Miles) ? 3960 : 6371;

        // Get Latitude Difference
        double dLat = toRadian(pos2.Y - pos1.Y);
        // Get Longitude Difference
        double dLon = toRadian(pos2.X - pos1.X);

        // accumulative difference in latitude and longitude
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(toRadian(pos1.Y)) * Math.Cos(toRadian(pos2.Y)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        // Output distance
        double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

        // Reference * Output = (Double) Distance between geolocations
        double d = R * c;

        return d;
    }

    /// <summary>
    /// Convert to Radians.
    /// </summary>
    /// <param name=”val”></param>
    /// <returns></returns>
    private static double toRadian(double val)
    {
        return (Math.PI / 180) * val;
    }

    /// <summary>
    /// Bubble Sort to find nearest neighbor
    /// </summary>
    /// <param name="user"></param>
    /// <param name="eventsToSearch"></param>
    /// <returns></returns>
    public static IEnumerable<Event> GetNearestNeighbor(User user, List<Event> eventsToSearch, double distance, DistanceType type = DistanceType.Kilometers)
    {
        var selectedPoint = user;
        
        // Precompute distances and positions
        double[] distances = new double[eventsToSearch.Count];
        for (int i = 0; i < eventsToSearch.Count; i++)
        {
            var eventLocation = eventsToSearch[i];
            distances[i] = Distance(selectedPoint, eventLocation);
        }

        // Bubble Sort with early termination
        bool swapped;
        for (int i = 0; i < eventsToSearch.Count; i++)
        {
            swapped = false;
            for (int j = 0; j < eventsToSearch.Count - 1 - i; j++)
            {
                if (distances[j] > distances[j + 1])
                {
                    // Swap distances
                    (distances[j], distances[j + 1]) = (distances[j + 1], distances[j]);

                    // Swap objects in list
                    (eventsToSearch[j], eventsToSearch[j + 1]) = (eventsToSearch[j + 1], eventsToSearch[j]);

                    swapped = true;
                }
            }
            // If no two elements were swapped by inner loop, break
            if (!swapped) break;
        }

        return eventsToSearch.Where(e => user.lastPartition <= e.date && Distance(selectedPoint, e) <= distance);
    }
}