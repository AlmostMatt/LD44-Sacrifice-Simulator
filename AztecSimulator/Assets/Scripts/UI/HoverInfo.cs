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
		GameObject canvasObject = Utilities.GetUIManager().gameObject;
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
        RectTransform uiTextRect = mUiInfo.GetComponent<RectTransform>();
		if (uiTextRect.rect.width == 0) {
			//Debug.Log("info text is temporarily offscreen");
			uiTextRect.anchoredPosition = new Vector2(0, mCanvasTransform.rect.height);
			return;
		}
        // The canvas might be scaled and this is a child of the canvas. Do math in screen coordinates.
        Rect hoverTextRect = LocalRectToScreenRect(uiTextRect);
        Rect objRect = LocalRectToScreenRect(gameObject.GetComponent<RectTransform>());
        // Place the uiText immediately above the relevant object
        Vector2 desiredPosition = new Vector2(
            objRect.x + objRect.width / 2,
            objRect.y + objRect.height + hoverTextRect.height / 2);
        // If this would push it off the top of the screen, put it below the object instead.
        if (desiredPosition.y + hoverTextRect.height / 2 > Screen.height)
        {
            desiredPosition.y = objRect.y - hoverTextRect.height / 2;
        }
        // Adjust X and Y in order to not go offscreen.
        Vector2 actualPosition = new Vector2(
			Mathf.Min(Mathf.Max(
                desiredPosition.x,
                hoverTextRect.width/2),
			Screen.width - (hoverTextRect.width/2)),
			Mathf.Min(Mathf.Max(
                desiredPosition.y,
                hoverTextRect.height/2),
				Screen.height- (hoverTextRect.height/2)));
        mUiInfo.transform.GetComponent<RectTransform>().anchoredPosition = ScreenPosToLocalPos(actualPosition, mCanvasTransform);
    }

    public static Rect LocalRectToScreenRect(RectTransform transform)
    {
        Vector2 worldSize = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (worldSize * 0.5f), worldSize);
    }

    public static Vector2 ScreenPosToLocalPos(Vector2 screenPos, RectTransform parentTransform)
    {
        // Invert the parent's offset and scale to convert to local coordinates.
        Vector2 relativePosition = screenPos - parentTransform.anchoredPosition;
        relativePosition.Scale(new Vector2(1f / parentTransform.localScale.x, 1f / parentTransform.localScale.y));
        return relativePosition;
    }
}