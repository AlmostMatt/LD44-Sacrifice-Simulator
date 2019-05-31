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
    public bool IsFleeting { get { return mTimeLeft != -1f; } }

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
	public string[] GetUIDescriptionStrings() {
		if(mLongDescOverride != null) {
			return(mLongDescOverride);
		}

		bool isOffer = mSatisfiedResult != null;
		string[] result = new string[2];
		if(IsFleeting)
		{
			result[0] = "";
            string description = (isOffer ? "OFFER: " + mSatisfiedResult.mName : "THREAT: " + mIgnoredResult.mName);
            result[1] = description + (mTimeLeft >= 0 ? string.Format(" ({0:0.0})", mTimeLeft) : "");
		}
        else
        {
            result[0] = "";
            result[1] = mSatisfiedResult.mName;
        }
		return result;
	}

	public string GetUIDescriptionIconName() {
        return mSatisfiedResult != null ? mSatisfiedResult.mIcon : mIgnoredResult.mIcon;
    }

    public bool IsSatisfied()
    {
        return GetEmptySlots().Count == 0;
    }

    public List<GameObject> GetAssociatedPeople()
    {
        GameObject relevantUiDemand = Utilities.GetUIManager().GetUiDemand(this);
        List<GameObject> associatedPeople = new List<GameObject>();
        for (int j = 0; j < 3; j++)
        {
            Draggable draggable = GetSlot(relevantUiDemand, j).GetComponentInChildren<Draggable>();
            if (draggable != null)
            {
                associatedPeople.Add(draggable.gameObject);
            }
        }
        return associatedPeople;
    }

    public List<int> GetEmptySlots()
    {
        GameObject relevantUiDemand = Utilities.GetUIManager().GetUiDemand(this);
        List<int> emptySlots = new List<int>();
        for (int j = 0; j < 3; j++)
        {
            Transform personSlot = GetSlot(relevantUiDemand, j);
            // A slot is empty if the child if it is active and doesn't have a draggable child
            if (personSlot.gameObject.activeInHierarchy && personSlot.GetComponentInChildren<Draggable>() == null)
            {
                emptySlots.Add(j);
            }
        }
        return emptySlots;
    }

    public List<int> GetRelevantSlots(Person person)
    {
        List<int> emptySlots = GetEmptySlots();
        List<int> relevantSlots = new List<int>();
        int demandSlotIndex = 0;
        foreach (Criterion criterion in mDemand.mCriteria)
        {
            bool isSatisfied = criterion.CheckSatisfaction(person);
            for (int j = 0; j < Mathf.Max(criterion.mCount, 1); j++)
            {
                if (isSatisfied && emptySlots.Contains(demandSlotIndex))
                {
                    relevantSlots.Add(demandSlotIndex);
                }
                demandSlotIndex++;
            }
        }
        return relevantSlots;
    }

	public void RenderTo(GameObject uiPrefab) {
        // TODO: associated dropped people on the demand
		uiPrefab.GetComponent<HoverInfo>().SetText(GetResultDescription());
        // Text
        string[] demandStrings = GetUIDescriptionStrings();
        Sprite icon = Utilities.GetSpriteManager().GetSprite(GetUIDescriptionIconName());
        uiPrefab.transform.Find("V/TextRow/Text1").GetComponent<Text>().text = demandStrings[0] + (icon == null ? "" : " ");
        uiPrefab.transform.Find("V/TextRow/Text2").GetComponent<Text>().text = (icon == null ? "" : " ") + demandStrings[1];
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
                        Debug.Log("WARNING: demand requires >3 sacrifices. Counts: "
                            + Utilities.ConcatStrings(mDemand.mCriteria.ConvertAll(c => c.mCount.ToString())));
                        continue;
                    }
                    Transform personSlot = GetSlot(uiPrefab, demandSlotIndex);
                    personSlot.gameObject.SetActive(true);
                    // Level
                    personSlot.Find("H/Level").GetComponent<Text>().enabled = criteria[i].mMinLevel != -1;
                    personSlot.Find("H/Level").GetComponent<Text>().text = criteria[i].mMinLevel.ToString();
                    // Profession
                    Sprite profession = Utilities.GetSpriteManager().GetSprite(criteria[i].GetProfession());
                    if (profession == null)
                    {
                        personSlot.Find("H/Profession/Image").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.2f);
                        personSlot.Find("H/Profession/Image").GetComponent<Image>().sprite = Utilities.GetSpriteManager().GetSprite("PERSON");
                    } else
                    {
                        personSlot.Find("H/Profession/Image").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.7f);
                        personSlot.Find("H/Profession/Image").GetComponent<Image>().sprite = profession;
                    }
                    // Attributes
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
                            attrImage.sprite = Utilities.GetSpriteManager().GetSprite(attributes[k]);
                        }
                    }
                    demandSlotIndex++;
                }
            }
        }
        // Disable the remaining slots
        for (int i = demandSlotIndex; i < 3; i++)
        {
            Transform personSlot = GetSlot(uiPrefab, i);
            personSlot.gameObject.SetActive(false);
        }
        // Show the filler if < 3 slots were used.
        GetSlot(uiPrefab, -1).gameObject.SetActive(demandSlotIndex < 3);
        GetSlot(uiPrefab, 3).gameObject.SetActive(demandSlotIndex < 3);
    }

    private Transform GetSlot(GameObject uiPrefab, int i)
    {
        return uiPrefab.transform.Find("V/PersonSlots").GetChild(i + 1);
    }
}