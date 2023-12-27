using System;

[Serializable]
public class EventDTO
{
    public Guid id { get; set; }
    public string? name { get; set; } = default;
    public string? description { get; set; } = default;
    public string address { get; set; } = default;
    public DateTime date { get; set; }
    public string? image { get; set; } = default;
    public EventDTO() {}
}