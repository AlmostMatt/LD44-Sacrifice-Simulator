using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ProfessionArea : MonoBehaviour, IDropHandler
{
    public Person.Attribute associatedProfession;

    public void OnDrop(PointerEventData eventData)
    {
        eventData.pointerDrag.transform.parent = transform.Find("G");
        // How to go from GameObject to Person?
        // Utilityies - get uimanager - change profession (gameobject, profession)
    }
}
