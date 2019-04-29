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
	private RectTransform mCanvasTransform;
	private Vector2 mDesiredPosition;

	public void Awake() {
		mUiInfo = Instantiate(uiInfoPrefab);
		mUiInfo.SetActive(false);
		GameObject canvasObject = GameObject.Find("UI Canvas");
		mCanvasTransform = canvasObject.GetComponent<RectTransform>();
		mUiInfo.transform.SetParent(canvasObject.transform, false);
		//Rect corners = RectTransformUtility.PixelAdjustRect(
		//	transform.GetComponent<RectTransform>(),
		//	canvasObject.GetComponent<Canvas>());
		//
		//mUiInfo.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 100f);
		// get coordinates of this relative
		// TODO: set pixel coordinates relative to the canvas. Use Min and Max to guarantee not too close to the edge. 
		SetText(detailedMessage);
		// TODO: maybe have canvas component for custom sorting
	}


	void OnDisable()
	{
		mUiInfo.SetActive(false);
	}

	void Update() {
		UpdatePosition();
	}

	public void SetText(string newText) {
		// I don't know why it happens, but mUiInfo seems to be temporarily null for new events
		if (mUiInfo != null) {
			mUiInfo.transform.GetChild(0).GetComponent<Text>().text = newText;
		} else {
			Debug.Log("MUI IS NULL");
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		mUiInfo.SetActive(true);
		mDesiredPosition = eventData.position + new Vector2(0f, 20f);
		UpdatePosition();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		mUiInfo.SetActive(false);
	}

	private void UpdatePosition() {
		// HACK
		// The size of this is 0,0 until it is enabled. So place it offscreen until it is enabled
		// and once it has a size do the math to make it not go off the edge of the screen
		Rect uiTextRect = mUiInfo.GetComponent<RectTransform>().rect;
		if (uiTextRect.width == 0) {
			//Debug.Log("info text is temporarily offscreen");
			mUiInfo.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, mCanvasTransform.rect.height);
			return;
		}
		Vector2 desiredPosition = new Vector2(
			Mathf.Min(Mathf.Max(
				mDesiredPosition.x, 
				uiTextRect.width/2),
			mCanvasTransform.rect.width - (uiTextRect.width/2)),
			Mathf.Min(Mathf.Max(
				mDesiredPosition.y,
				uiTextRect.height/2),
				mCanvasTransform.rect.height- (uiTextRect.height/2)));
		mUiInfo.transform.GetComponent<RectTransform>().anchoredPosition = desiredPosition - mCanvasTransform.anchoredPosition;
		// set global position relative to mouse
	}
}