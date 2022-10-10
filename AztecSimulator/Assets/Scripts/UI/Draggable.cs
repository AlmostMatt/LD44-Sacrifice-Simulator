using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Implements drag handlers. Resets to original location if the parent was not changed by an OnDrop handler.
// Also, other scripts may call Add/RemoveRelevantGrid from OnPointerEnter/Exit.
// If this happens, an empty game object will be added as a child of the grid in an index close to the cursor.
// The idea is that this will cause grid elements to move away from where this would be dropped.
public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // PlaceholderObject should be an empty game object with a RectTransform.
    public GameObject placeholderObject;

    private Transform originalParent;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private GridLayoutGroup relevantGrid = null;
    private Transform placeholder;
    private Transform canvas;

    public void Awake()
    {
        if (GetComponent<CanvasGroup>() == null)
        {
            gameObject.AddComponent<CanvasGroup>();
        }
        placeholder = Instantiate(placeholderObject).transform;
        canvas = Utilities.GetCanvas();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Disable raycast blocking while being dragged so that other objects can receive mouseOver and onDrop events
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        // Unattach this object. If the parent was a profession area, add a placeholder.
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;
        transform.SetParent(canvas, true);
        transform.localScale = new Vector3(1f, 1f, 1f); // Undo any scaling from DemandSlot
        GridLayoutGroup grid = originalParent.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            AddRelevantGrid(grid, eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        UpdatePlaceholderIndex(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        RemoveRelevantGrid(relevantGrid);
        // If no OnDrop handler changed the parent of this object, put it where it was previously.
        if (transform.parent == Utilities.GetCanvas())
        {
            transform.SetParent(originalParent, false);
            transform.localPosition = originalPosition;
            transform.localScale = originalScale;
        }
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void AddRelevantGrid(GridLayoutGroup grid, PointerEventData eventData)
    {
        relevantGrid = grid;
        placeholder.SetParent(relevantGrid.transform, false);
        placeholder.localPosition = (Vector3)eventData.position - grid.transform.position;
        UpdatePlaceholderIndex(eventData);
    }

    public void RemoveRelevantGrid(GridLayoutGroup grid)
    {
        if (relevantGrid == grid)
        {
            relevantGrid = null;
            placeholder.SetParent(canvas, false);
        }
    }

    public void UpdatePlaceholderIndex(PointerEventData eventData)
    {
        if (relevantGrid != null)
        {
            int index = Utilities.GetIndexOfClosestChild(relevantGrid.transform, eventData.position);
            placeholder.SetSiblingIndex(index);
        }
    }

    public void OnDestroy()
    {
        if (placeholder) Destroy(placeholder.gameObject);
    }
}
