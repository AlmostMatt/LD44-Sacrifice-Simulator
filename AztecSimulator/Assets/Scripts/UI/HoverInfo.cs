using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;// Required when using Event data.

public class HoverInfo :  MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public GameObject infoObject;
	// TODO: generalize this so that it can create its own child game object and only needs info text as a property
	public void OnPointerEnter(PointerEventData eventData)
	{
		infoObject.SetActive(true);
	}
	public void OnPointerExit(PointerEventData eventData)
	{
		infoObject.SetActive(false);
	}
}