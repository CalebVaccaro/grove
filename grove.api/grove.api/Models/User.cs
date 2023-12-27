using grove.DTOModels;

namespace grove;

public class User
{
    public Guid id { get; set; }
    public string? name { get; set; } = default;
    public List<Guid> createdEventIds { get; set; } = null;
    public List<Guid> matchedEventIds { get; set; } = null;
    public string address { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    // last query partition
    public DateTime lastPartition { get; set; }
}
