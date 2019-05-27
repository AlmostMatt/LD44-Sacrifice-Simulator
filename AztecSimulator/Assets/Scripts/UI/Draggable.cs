using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Transform originalParent;
    private Vector3 originalPosition;

    public void Start()
    {
        if (GetComponent<CanvasGroup>() == null)
        {
            gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        transform.parent = Utilities.GetCanvas();
        // Disable raycast blocking while being dragged so that other objects can receive mouseOver and onDrop events
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // If no OnDrop handler changed the parent of this object, put it where it was previously.
        if (transform.parent == Utilities.GetCanvas())
        {
            transform.parent = originalParent;
            transform.localPosition = originalPosition;
        }
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
