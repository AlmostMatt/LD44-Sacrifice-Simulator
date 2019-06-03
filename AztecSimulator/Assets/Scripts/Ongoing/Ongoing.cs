using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ongoing : IRenderable
{
	public string mIconName;
	public string mTitle;
	public string mDescription;
	public float mDuration;
	public bool mIsGood; // TODO: generalize to good/bad/neutral

	public Ongoing (string iconName, string title, string description, float duration, bool isGood)
	{
		mIconName = iconName;
		mTitle = title;
		mDescription = description;
		mDuration = duration;
		mIsGood = isGood;
	}

	public void RenderTo(GameObject uiOngoing) {
		if (mIconName != "") {
			uiOngoing.transform.Find("Icon").GetComponent<Image>().sprite = Utilities.GetSpriteManager().GetSprite(mIconName);
			uiOngoing.transform.Find("Icon").gameObject.SetActive(true);
		} else {
			uiOngoing.transform.Find("Icon").gameObject.SetActive(false);
		}
		uiOngoing.transform.Find("VGroup/Text1").GetComponent<Text>().text = mTitle;
		uiOngoing.transform.Find("VGroup/Text2").GetComponent<Text>().text = string.Format("{0:0.0}s", mDuration);
		uiOngoing.GetComponent<HoverInfo>().SetText(mDescription.ToString());
		// trying lighter and lighter shades, because its a multiplied by a gray image
		//uiOngoing.GetComponent<Image>().color = mIsGood ? new Color(77f/255f, 158f/255f, 115/255f) : new Color(158f/255f,53f/255f,64f/255f);
		//uiOngoing.GetComponent<Image>().color = mIsGood ? new Color(99f/255f, 204f/255f, 146/255f) : new Color(193f/255f,65/255f,80f/255f);
		//uiOngoing.GetComponent<Image>().color = mIsGood ? new Color(118f/255f, 242f/255f, 172/255f) : new Color(226f/255f,77f/255f,97f/255f);
		//uiOngoing.GetComponent<Image>().color = mIsGood ? new Color(118f/255f, 242f/255f, 172/255f) : new Color(244f/255f,83f/255f,104f/255f);
		uiOngoing.GetComponent<Image>().color = mIsGood ? new Color(118f/255f, 242f/255f, 172/255f) : new Color(255f/255f,89f/255f,111f/255f);
	}
}

