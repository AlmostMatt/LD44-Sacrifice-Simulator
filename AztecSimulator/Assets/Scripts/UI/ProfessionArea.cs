using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProfessionArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public Person.Attribute associatedProfession;
    public GameObject placeholderObject;

    // When mousing over an area while dragging, attach a placeholder.
    public void OnPointerEnter(PointerEventData eventData)
    {
        AttachPlaceholder(eventData);
    }

    // When mousing away from an area while dragging, detach the placeholder.
    public void OnPointerExit(PointerEventData eventData)
    {
        DetachPlaceholder(eventData);
    }

    // When dropping an object here, attempt to place them in the grid at the index of the placeholder.
    // Also detach the corresponding placeholder.
    public void OnDrop(PointerEventData eventData)
    {
        Transform contentGrid = transform.Find("Scrollable/Viewport/Content (G)");
        int index = Utilities.GetIndexOfClosestChild(contentGrid, eventData.position);
        DetachPlaceholder(eventData);
        bool changeSuccessful = Utilities.GetUIManager().OnChangeProfession(eventData.pointerDrag, associatedProfession);
        if (changeSuccessful)
        {
            eventData.pointerDrag.transform.localPosition = Vector3.zero;
            eventData.pointerDrag.transform.SetParent(contentGrid, false);
            eventData.pointerDrag.transform.SetSiblingIndex(index);
        }
    }

    private void AttachPlaceholder(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Draggable draggable = eventData.pointerDrag.GetComponent<Draggable>();
            if (draggable != null)
            {
                draggable.AddRelevantGrid(transform.Find("Scrollable/Viewport/Content (G)").GetComponent<GridLayoutGroup>(), eventData);
            }
        }
    }

    private void DetachPlaceholder(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Draggable draggable = eventData.pointerDrag.GetComponent<Draggable>();
            if (draggable != null)
            {
                draggable.RemoveRelevantGrid(transform.Find("Scrollable/Viewport/Content (G)").GetComponent<GridLayoutGroup>());
            }
        }
    }
}
