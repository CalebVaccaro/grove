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
        private string GROVE_API_URL;

        public void Awake()
        {
            Environment.SetEnvironmentVariable("GROVE_API_URL", "http://localhost:5103");
            GROVE_API_URL = System.Environment.GetEnvironmentVariable("GROVE_API_URL");
            Debug.Log(GROVE_API_URL);
        }
        
        public async Task<UserDTO> CreateUser(UserDTO user)
        {
            return await ApiService.Post<UserDTO>($@"{GROVE_API_URL}/users", user);
        }

        public async Task<List<EventDTO>> GetEvents()
        {
            return await ApiService.Get<List<EventDTO>>($@"{GROVE_API_URL}/events");
        }
        
        public async Task<IAsyncResult> CreateMatch(Guid userId, Guid eventId)
        {
            return await ApiService.Get<IAsyncResult>($@"{GROVE_API_URL}/matches/{userId}/{eventId}");
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
            
            //await CreateUser(user);
            var events = await GetEvents();
            OnGetEvents?.Invoke(events);
            
            //var match = await CreateMatch(user.id, events[0].id);
        }
    }
}