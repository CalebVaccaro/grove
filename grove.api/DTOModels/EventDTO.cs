namespace grove.DTOModels;

public class EventDTO
{
    public Guid id { get; set; }
    public string? name { get; set; } = default;
    public string? description { get; set; } = default;
    public double X { get; set; }
    public double Y { get; set; }
    public DateTime date { get; set; }
    public EventDTO() {}
    public EventDTO(Event evnt)
    {
        this.id = evnt.id;
        this.name = evnt.name;
        this.description = evnt.description;
        this.X = evnt.X;
        this.Y = evnt.Y;
        this.date = evnt.date;
    }
}