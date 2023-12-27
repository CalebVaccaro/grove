using System.Collections.Generic;
using UnityEngine;

namespace Grove.UI
{
    public class EventController : MonoBehaviour
    {
        public List<string> list = new List<string>();
        public SwipeEffect card;
        public string evnt;
        public int index = 0;
        
        public void Start()
        {
            // move to next card
            evnt = list[index];
            card.OnSwipeComplete.AddListener(OnSwipeComplete);
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
            
            // Reload Card
            card.transform.localPosition = new Vector3(0,101,0);
            Debug.Log("Enable Card");
            card.gameObject.SetActive(true);
        }
    }
}