using System;
using System.Collections.Generic;

public class UserDTO
{
    public Guid id { get; set; }
    public string? name { get; set; } = default;
    public List<Guid> createdEventIds { get; set; }
    public List<Guid> matchedEventIds { get; set; }
    public string address { get; set; }
    
    public UserDTO() {}
}