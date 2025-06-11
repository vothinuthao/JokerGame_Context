using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DragAndDropUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    public UnityEvent onDragExit;
    public RectTransform dragArea;
    
    public RectTransform playArea; // RectTransform của ô vuông "Play"
    public RectTransform discardArea; // RectTransform của ô vuông "Discard"
    
    public UnityEvent onPlay; // Sự kiện khi kéo vào ô "Play"
    public UnityEvent onDiscard; // Sự kiện khi kéo vào ô "Discard"
    public List<Transform> listTranform = new List<Transform>();

    private Vector2 offset;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(dragArea, Input.mousePosition))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dragArea, Input.mousePosition, eventData.pressEventCamera, out offset);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragArea != null)
        {
            Vector2 pointerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (var trans in listTranform)
            {
                Vector3 newPosition = pointerPosition - offset;
                newPosition.z = transform.position.z; // Giữ nguyên giá trị Z
                trans.transform.position = newPosition;
            }
            
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(playArea, Input.mousePosition))
        {
            onPlay.Invoke(); // Gọi sự kiện "Play"
            Debug.Log("Playyyy");
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(discardArea, Input.mousePosition))
        {
            onDiscard.Invoke(); // Gọi sự kiện "Discard"
            Debug.Log("Discard");
        }
        else if (!RectTransformUtility.RectangleContainsScreenPoint(dragArea, Input.mousePosition))
        {
            onDragExit.Invoke(); // Gọi sự kiện khi kéo ra khỏi vùng
            
        }
    }
}