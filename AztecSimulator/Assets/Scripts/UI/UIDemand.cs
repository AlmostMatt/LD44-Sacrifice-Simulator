﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIDemand : MonoBehaviour, IDropHandler
{
    private const float UI_PERSON_SCALE_WHEN_DROPPED = 0.75f;

    public void OnDrop(PointerEventData eventData)
    {
        List<int> relevantSlots = Utilities.GetUIManager().OnDropPersonOnDemand(eventData.pointerDrag, gameObject);
        if (relevantSlots.Count > 0)
        {
            int closestIndex = 0;
            float minDD = Mathf.Infinity;
            foreach (int i in relevantSlots)
            {
                Vector2 childPos = GetSlot(i).position;
                float DD = (eventData.position - childPos).sqrMagnitude;
                if (DD < minDD)
                {
                    minDD = DD;
                    closestIndex = i;
                }
            }
            Transform personSlot = GetSlot(closestIndex);
            eventData.pointerDrag.transform.SetParent(personSlot, false);
            eventData.pointerDrag.transform.localPosition = Vector3.zero;
            eventData.pointerDrag.transform.localScale = new Vector3(
                UI_PERSON_SCALE_WHEN_DROPPED, UI_PERSON_SCALE_WHEN_DROPPED, UI_PERSON_SCALE_WHEN_DROPPED);
            Utilities.GetUIManager().MaybeSacrifice(gameObject);
        }
    }

    private Transform GetSlot(int i)
    {
        return transform.Find("V/PersonSlots").GetChild(i + 1);
    }
}
