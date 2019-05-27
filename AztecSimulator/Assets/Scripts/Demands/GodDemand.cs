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
	public string[] GetUIDescriptionStrings(List<Person> selectedPeople) {
		if(mLongDescOverride != null) {
			return(mLongDescOverride);
		}

		bool isOffer = mSatisfiedResult != null;
		string[] result = new string[2];
		if(IsFleeting)
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

    public bool IsSatisfied()
    {
        return GetEmptySlots().Count == 0;
    }

    public List<GameObject> GetAssociatedPeople()
    {
        Transform relevantUiDemand = Utilities.GetUIManager().GetUiDemand(this).transform;
        List<GameObject> associatedPeople = new List<GameObject>();
        for (int j = 0; j < 3; j++)
        {
            Draggable draggable = relevantUiDemand.Find("V/PersonSlots").GetChild(j).GetComponentInChildren<Draggable>();
            if (draggable != null)
            {
                associatedPeople.Add(draggable.gameObject);
            }
        }
        return associatedPeople;
    }

    public List<int> GetEmptySlots()
    {
        Transform relevantUiDemand = Utilities.GetUIManager().GetUiDemand(this).transform;
        List<int> emptySlots = new List<int>();
        for (int j = 0; j < 3; j++)
        {
            Transform personSlot = relevantUiDemand.Find("V/PersonSlots").GetChild(j);
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
        List<Person> selectedPeople = new List<Person>();// Utilities.GetSelectedPeople();
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
                        Debug.Log("WARNING: demand requires >3 sacrifices. Counts: "
                            + Utilities.ConcatStrings(mDemand.mCriteria.ConvertAll(c => c.mCount.ToString())));
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
                            attrImage.sprite = Utilities.GetSpriteManager().GetSprite(attributes[k]);
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