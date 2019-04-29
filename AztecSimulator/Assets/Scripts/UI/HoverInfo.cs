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
	// TODO: generalize this so that it can create its own child game object and only needs info text as a property
	public void SetText(string newText) {
		Debug.Log("set");
		if (mUiInfo != null) {
			Debug.Log(mUiInfo.transform.childCount);
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