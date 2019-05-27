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
            eventData.pointerDrag.transform.parent = transform.Find("G");
        }
    }
}
