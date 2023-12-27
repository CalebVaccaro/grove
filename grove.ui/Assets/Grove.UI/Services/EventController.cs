using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EAFU;
using UnityEngine;

namespace Grove.UI.Services
{
    public class EventController : MonoBehaviour
    {
        public Action<List<EventDTO>> OnGetEvents;

        public async Task<UserDTO> CreateUser(UserDTO user)
        {
            return await ApiService.Post<UserDTO>("http://localhost:5103/users", user);
        }

        public async Task<List<EventDTO>> GetEvents()
        {
            return await ApiService.Get<List<EventDTO>>("http://localhost:5103/events");
        }
        
        public async Task<IAsyncResult> CreateMatch(Guid userId, Guid eventId)
        {
            return await ApiService.Get<IAsyncResult>($"http://localhost:5103/matches/{userId}/{eventId}");
        }
        
        public async void Start()
        {
            var user = new UserDTO()
            {
                address = "1234 Main St",
                createdEventIds = new List<Guid>(),
                matchedEventIds = new List<Guid>(),
                name = "Test User"
            };
            
            await CreateUser(user);
            var events = await GetEvents();
            OnGetEvents?.Invoke(events);
            
            var match = await CreateMatch(user.id, events[0].id);
        }
    }
}