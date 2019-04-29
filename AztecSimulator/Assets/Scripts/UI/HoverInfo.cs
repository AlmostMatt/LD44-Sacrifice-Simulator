using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class HoverInfo :  MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public GameObject uiInfoPrefab;
	public string detailedMessage = "this can be set programmatically";

	private GameObject mUiInfo;

	public void Start() {
		mUiInfo = Instantiate(uiInfoPrefab);
		mUiInfo.SetActive(false);
		mUiInfo.transform.SetParent(GameObject.Find("UI Canvas").transform, false);
		// TODO: set pixel coordinates relative to the canvas. Use Min and Max to guarantee not too close to the edge. 
		SetText(detailedMessage);
		// TODO: maybe have canvas component for custom sorting
	}


	void OnDisable()
	{
		mUiInfo.SetActive(false);
	}

	public void SetText(string newText) {
		// I don't know why it happens, but mUiInfo seems to be temporarily null for new events
		if (mUiInfo != null) {
			mUiInfo.transform.GetChild(0).GetComponent<Text>().text = newText;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		mUiInfo.SetActive(true);
		// set global position relative to mouse
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		mUiInfo.SetActive(false);
	}
}