using grove.DTOModels;

namespace grove;

public class Event
{
    public Guid id { get; set; }
    public string? name { get; set; } = default;
    public string? description { get; set; } = default;
    public double X { get; set; }
    public double Y { get; set; }
    public DateTime date { get; set; }
    public string? Secret { get; set; }
}