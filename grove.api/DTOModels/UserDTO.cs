namespace grove.DTOModels;

public class UserDTO
{
    public string? name { get; set; } = default;
    public List<Guid> createdEventIds { get; set; }
    public List<Guid> matchedEventIds { get; set; }
    public string address { get; set; }
    
    public UserDTO() {}
    public UserDTO(User user)
    {
        this.name = user.name;
        this.createdEventIds = user.createdEventIds;
        this.matchedEventIds = user.matchedEventIds;
        this.address = user.address;
    }
}