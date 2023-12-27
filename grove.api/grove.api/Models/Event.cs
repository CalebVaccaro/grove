using grove.DTOModels;

namespace grove;

public class Event
{
    public Guid id { get; set; }

    public string? name { get; set; } = default;
    public string? description { get; set; } = default;
    // GIS
    public string address { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    // key to partition already swiped on event
    // event does not show up in the next partition for the user
    public DateTime date { get; set; }
    // hold event image
    public string? Image { get; set; }
}