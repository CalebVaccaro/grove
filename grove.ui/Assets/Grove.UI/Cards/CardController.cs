    using System;
    using System.Collections.Generic;
    using Grove.UI.Services;
    using UnityEngine;

namespace Grove.UI
{
    public class CardController : MonoBehaviour
    {
        public List<EventDTO> list = new List<EventDTO>();
        public SwipeEffect card;
        public EventDTO evnt;
        public int index = 0;
        public Action<EventDTO> OnNextEventCard;

        [SerializeField] private EventController EventController;
        
        public void Start()
        {
            card.gameObject.SetActive(false);
        }
        
        public void OnEnable()
        {
            EventController.OnGetEvents += OnGetEvents;
        }

        private void OnGetEvents(List<EventDTO> events)
        {
            // move to next card
            list = events;
            evnt = list[index];
            card.OnSwipeComplete.AddListener(OnSwipeComplete);
            card.gameObject.SetActive(true);
            OnNextEventCard.Invoke(evnt);
        }

        private void OnSwipeComplete(bool matched)
        {
            Debug.Log("Matched: " + matched);
            
            if (index >= list.Count - 1)
            {
                Debug.Log("No more events");
                return;
            }
            
            // move to next card
            evnt = list[++index];
            OnNextEventCard.Invoke(evnt);
            
            // Reload Card
            card.transform.localPosition = new Vector3(0,101,0);
            Debug.Log("Enable Card");
            card.gameObject.SetActive(true);
        }
    }
}