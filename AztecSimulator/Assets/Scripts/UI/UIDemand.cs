using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIDemand : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        List<int> relevantSlots = Utilities.GetUIManager().OnDropPersonOnDemand(eventData.pointerDrag, gameObject);
        if (relevantSlots.Count > 0)
        {
            // TODO: when there are multiple relevant slots, pick the slot closer to the cursor
            Transform personSlot = transform.Find("V/PersonSlots").GetChild(relevantSlots[0]+1);
            eventData.pointerDrag.transform.SetParent(personSlot, false);
            eventData.pointerDrag.transform.localPosition = Vector3.zero;
            Utilities.GetUIManager().MaybeSacrifice(gameObject);
        }
    }
}
