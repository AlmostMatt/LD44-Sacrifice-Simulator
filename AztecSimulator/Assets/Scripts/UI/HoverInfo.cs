using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;// Required when using Event data.

public class HoverInfo :  MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public GameObject infoObject;
	public void OnPointerEnter(PointerEventData eventData)
	{
		infoObject.SetActive(true);
	}
	public void OnPointerExit(PointerEventData eventData)
	{
		infoObject.SetActive(false);
	}
}