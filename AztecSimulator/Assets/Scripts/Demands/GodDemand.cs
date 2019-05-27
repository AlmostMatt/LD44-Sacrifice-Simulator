using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodDemand : IRenderable
{
	private static int sId = 0;

	public int mId;
	public SacrificeDemand mDemand;
	public SacrificeResult mSatisfiedResult;
	public SacrificeResult mIgnoredResult;

	public float mTimeLeft = -1f;
	public bool mIsRenewable = false;
	public int mNumBuys = 0;

	public string[] mLongDescOverride;

	public GodDemand(SacrificeDemand demand, SacrificeResult satisfiedResult, SacrificeResult ignoredResult)
	{
		mId = ++sId;

		mDemand = demand;
		mSatisfiedResult = satisfiedResult;
		mIgnoredResult = ignoredResult;
	}

	public string GetResultDescription() {
		// todo: consider listing both effect and cost
		return (mSatisfiedResult == null ? mIgnoredResult : mSatisfiedResult).mDescription;
	}

	public string GetShortDescription()
	{
		return(mDemand.GetShortDescription());
	}

	// Returns a list of two strings. one before the icon, and one after the icon.
	public string[] GetUIDescriptionStrings(List<Person> selectedPeople) {
		if(mLongDescOverride != null) {
			return(mLongDescOverride);
		}

		bool isFleeting = mTimeLeft >= 0;
		bool isOffer = mSatisfiedResult != null;
		string[] result = new string[2];
		if(isFleeting)
		{
			result[0] = isOffer ? "OFFER: " + mSatisfiedResult.mName : "THREAT: " + mIgnoredResult.mName;
			result[1] = mTimeLeft >= 0 ? string.Format("({0:0.0})", mTimeLeft) : "";
		}
        else
        {
            result[0] = mSatisfiedResult.mName;
            result[1] = "";
        }
		return result;
	}

	public string GetUIDescriptionIconName() {
        // TODO: have relevant icons
        return ""; //  "ATTACK";
	}

	public void RenderTo(GameObject uiPrefab) {
        // TODO: associated dropped people on the demand
        List<Person> selectedPeople = new List<Person>();// Utilities.GetSelectedPeople();
		// Hack: The GameObject stores demand ID in order to make selected demand lookup possible
		uiPrefab.name = mId.ToString();
		uiPrefab.GetComponent<HoverInfo>().SetText(GetResultDescription());
        // Text
        string[] demandStrings = GetUIDescriptionStrings(selectedPeople);
        uiPrefab.transform.Find("V/TextRow/Text1").GetComponent<Text>().text = demandStrings[0];
        uiPrefab.transform.Find("V/TextRow/Text2").GetComponent<Text>().text = demandStrings[1];
        Sprite icon = Utilities.GetSpriteManager().GetSprite(GetUIDescriptionIconName());
        uiPrefab.transform.Find("V/TextRow/Icon").gameObject.SetActive(icon != null);
        uiPrefab.transform.Find("V/TextRow/Icon").GetComponent<Image>().sprite = (icon);
        // Criteria
        List<Criterion> criteria = mDemand.mCriteria;
        int demandSlotIndex = 0;
        for (int i = 0; i < 3; i++)
        {
            if (i < criteria.Count)
            {
                if (criteria[i].mMinAge != -1)
                {
                    Debug.Log("WARNING: age criteria: " + criteria[i].mMinAge);
                }
                for (int j = 0; j < Mathf.Max(criteria[i].mCount, 1); j++)
                {
                    if (demandSlotIndex >= 3)
                    {
                        Debug.Log("WARNING: demand requires >3 sacrifices.");
                        continue;
                    }
                    Transform personSlot = uiPrefab.transform.Find("V/PersonSlots").GetChild(demandSlotIndex);
                    personSlot.gameObject.SetActive(true);

                    personSlot.Find("H/Level").GetComponent<Text>().enabled = criteria[i].mMinLevel != -1;
                    personSlot.Find("H/Level").GetComponent<Text>().text = criteria[i].mMinLevel.ToString();
                    Sprite profession = Utilities.GetSpriteManager().GetSprite(criteria[i].GetProfession());
                    personSlot.Find("H/Profession/Image").GetComponent<Image>().enabled = profession != null;
                    personSlot.Find("H/Profession/Image").GetComponent<Image>().sprite = profession;
                    //
                    List<Person.Attribute> attributes = criteria[i].mAttributes.FindAll(attr => attr.GetAttrType() != Person.AttributeType.PROFESSION);
                    if (attributes.Count > 2)
                    {
                        Debug.Log("WARNING: requiring 3 attributes: " + attributes.ToString());
                    }
                    for (int k = 0; k < 2; k++)
                    {
                        Image attrImage = personSlot.Find("H/V").GetChild(k).GetComponent<Image>();
                        attrImage.enabled = k < attributes.Count;
                        if (k < attributes.Count)
                        {
                            // TODO: custom image
                        }
                    }
                    demandSlotIndex++;
                }
            }
        }
        for (int i = demandSlotIndex; i < 3; i++)
        {
            Transform personSlot = uiPrefab.transform.Find("V/PersonSlots").GetChild(i);
            personSlot.gameObject.SetActive(false);
        }
    }
}