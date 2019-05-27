using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ProfessionArea : MonoBehaviour, IDropHandler
{
    public Person.Attribute associatedProfession;

    public void OnDrop(PointerEventData eventData)
    {
        bool changeSuccessful = Utilities.GetUIManager().OnChangeProfession(eventData.pointerDrag, associatedProfession);
        if (changeSuccessful)
        {
            eventData.pointerDrag.transform.localPosition = Vector3.zero;
            eventData.pointerDrag.transform.SetParent(transform.Find("Scrollable/Viewport/Content (G)"), false);
        }
    }
}
