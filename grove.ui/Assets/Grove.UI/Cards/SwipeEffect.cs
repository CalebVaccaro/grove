using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeEffect : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    private Vector3 _initialPosition;
    private float _distanceMoved;
    private bool _swipeLeft;
    public UnityEvent<bool> OnSwipeComplete;
    
    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition = new Vector2(transform.localPosition.x+eventData.delta.x,transform.localPosition.y);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _initialPosition = transform.localPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _distanceMoved = Mathf.Abs(transform.localPosition.x - _initialPosition.x);
        if(_distanceMoved<0.4*Screen.width)
        {
            transform.localPosition = _initialPosition;
        }
        else
        {
            if (transform.localPosition.x > _initialPosition.x)
            {
                _swipeLeft = false;
                
            }
            else
            {
                _swipeLeft = true;
            }
            StartCoroutine(MovedCard());
        }
    }

    private IEnumerator MovedCard()
    {
        float time = 0;
        while (GetComponent<Image>().color != new Color(1, 1, 1, 0))
        {
            Debug.Log("Moving");
            time += Time.deltaTime;
            if (_swipeLeft)
            {
                Debug.Log("Moving Left");
                transform.localPosition = new Vector3(Mathf.SmoothStep(transform.localPosition.x,
                    transform.localPosition.x-Screen.width,time),transform.localPosition.y,0);
            }
            else
            {
                Debug.Log("Moving Right");
                transform.localPosition = new Vector3(Mathf.SmoothStep(transform.localPosition.x,
                    transform.localPosition.x+Screen.width,time),transform.localPosition.y,0);
            }
            GetComponent<Image>().color = new Color(1,1,1,Mathf.SmoothStep(1,0,4*time));
            yield return null;
        }

        // reset color
        OnSwipeComplete?.Invoke(!_swipeLeft);
        GetComponent<Image>().color = new Color(1,1,1,1);
    }
}
