namespace grove.DTOModels;

public class EventDTO
{
    public string? name { get; set; } = default;
    public string? description { get; set; } = default;
    public string address { get; set; } = default;
    public DateTime date { get; set; }
    public string? image { get; set; } = default;
    public EventDTO() {}
    public EventDTO(Event evnt)
    {
        this.name = evnt.name;
        this.description = evnt.description;
        this.date = evnt.date;
        this.image = evnt.Image;
        this.address = evnt.address;
    }
}