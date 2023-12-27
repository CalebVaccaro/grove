using System;
using UnityEngine;
using UnityEngine.UI;

namespace Grove.UI
{
    public class CardsUI : MonoBehaviour
    {
        public CardController cardController;
        public Text eventName;
        public Text eventDescription;
        public Text eventAddress;
        public Text eventDate;
        public Text eventTime;

        public void OnEnable()
        {
            cardController.OnNextEventCard += OnNextEventCard;
        }

        private void OnNextEventCard(EventDTO obj)
        {
            eventName.text = obj.name;
            eventDescription.text = obj.description;
            eventAddress.text = obj.address;
            eventDate.text = obj.date.ToShortDateString();
            eventTime.text = obj.date.TimeOfDay.ToString();
        }

        public void OnDisable()
        {
            cardController.OnNextEventCard -= OnNextEventCard;
        }
    }
}