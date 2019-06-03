using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProfessionArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public PersonAttribute associatedProfession;
    public GameObject placeholderObject;

    public void Awake()
    {
        this.name = associatedProfession.ToString();
        // Update visuals
        transform.Find("Top bar/Icon").GetComponent<Image>().sprite = Utilities.GetSpriteManager().GetSprite(associatedProfession);
        Color32 profColor = associatedProfession.GetColor();
        // Color the background based on the profession's associated color
        GetComponent<Image>().color = new Color32(profColor.r, profColor.g, profColor.b, 30);
        // TODO: use the associated profession's color for icon color once the icons are white
        //transform.Find("Top bar/Icon").GetComponent<Image>().color = ;
        string labelString = string.Format(
            "{0} ({1} + {2}/lvl)",
            associatedProfession.GetDescription(),
            associatedProfession.GetEfficiencyBase(),
            associatedProfession.GetEfficiencyPerLevel());
        transform.Find("Top bar/Text").GetComponent<Text>().text = labelString;
    }

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

    // When dropping an object here, detach the placeholder and place the person at the index closest to the mouse.
    public void OnDrop(PointerEventData eventData)
    {
        Transform contentGrid = transform.Find("Scrollable/Viewport/Content (G)");
        DetachPlaceholder(eventData);
        bool changeSuccessful = Utilities.GetUIManager().OnChangeProfession(eventData.pointerDrag, associatedProfession);
        if (changeSuccessful)
        {
            eventData.pointerDrag.transform.localPosition = Vector3.zero;
            eventData.pointerDrag.transform.SetParent(contentGrid, false);
            int index = Utilities.GetIndexOfClosestChild(contentGrid, eventData.position);
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
