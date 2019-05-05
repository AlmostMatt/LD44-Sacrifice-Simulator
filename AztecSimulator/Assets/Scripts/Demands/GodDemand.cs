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

	// Returns a list of strings. Two per row, one before the icon, and one after the icon.
	public string[] GetUIDescriptionStrings(List<Person> selectedPeople) {
		if(mLongDescOverride != null) {
			return(mLongDescOverride);
		}

		bool isFleeting = mTimeLeft >= 0;
		bool isOffer = mSatisfiedResult != null;
		int numLinesPreCriteria = (isFleeting ? 6 : 4);
		string[] result = new string[numLinesPreCriteria + (2 * mDemand.NumCriteria)];

		int linesIdx = 0;
		if(isFleeting)
		{
			result[linesIdx++] = isOffer ? "GOD OFFERS " : "GOD THREATENS ";
			result[linesIdx++] = mTimeLeft >= 0 ? string.Format("({0:0.0})", mTimeLeft) : "";
		}

		result[linesIdx++] = isOffer ? mSatisfiedResult.mName : mIgnoredResult.mName;
		result[linesIdx++] = "";
		result[linesIdx++] = isOffer ? "In exchange for" : "Unless you pay";
		result[linesIdx++] = "";

		for(int i = 0; i < mDemand.NumCriteria; i++)
		{
			Criterion c = mDemand.mCriteria[i];
			bool satisfied = c.CheckSatisfaction(selectedPeople);
			result[numLinesPreCriteria + (2 * i)] = Utilities.ColorString(c.GetPrefixString(), "green", satisfied);
			result[numLinesPreCriteria + (2 * i) + 1] = Utilities.ColorString(c.GetSuffixString(), "green", satisfied);
		}

		return result;
	}

	public string[] GetUIDescriptionIconNames() {
		Person.Attribute[] demandAttributes = mDemand.GetUIDescriptionIcons(); 

		int numLinesPreCriteria = mTimeLeft >= 0 ? 3 : 2;
		string[] attributes = new string[numLinesPreCriteria + demandAttributes.Length];

		for(int i = 0; i < numLinesPreCriteria; ++i) { // TODO
			attributes[i] = "";
		}

		for(int i = 0; i < demandAttributes.Length; ++i)
		{
			attributes[i + numLinesPreCriteria] = demandAttributes[i].ToString();
		}

		return(attributes);
	}

	public void RenderTo(GameObject uiPrefab) {
		List<Person> selectedPeople = Utilities.GetSelectedPeople();
		// Hack: The GameObject stores demand ID in order to make selected demand lookup possible
		uiPrefab.name = mId.ToString();
		uiPrefab.GetComponent<HoverInfo>().SetText(GetResultDescription());
		string[] demandStrings = GetUIDescriptionStrings(selectedPeople);
		string[] demandIconNames = GetUIDescriptionIconNames();
		int numRows = Mathf.Min(demandStrings.Length / 2, demandIconNames.Length);
		Transform uiDemandVGroup = uiPrefab.transform.Find("VGroup");
		for (int rowI = 0; rowI < Mathf.Max(numRows, uiDemandVGroup.childCount); rowI++) {
			Transform row;
			if (rowI >= uiDemandVGroup.childCount) {
				row = GameObject.Instantiate(uiDemandVGroup.GetChild(0));
				row.SetParent(uiDemandVGroup, false);
			} else {
				row = uiDemandVGroup.GetChild(rowI);
			}
			row.gameObject.SetActive(rowI < numRows);
			if (rowI < numRows) {
				row.GetChild(0).GetComponent<Text>().text = demandStrings[2 * rowI];
				row.GetChild(1).GetComponent<Image>().sprite = Utilities.GetSpriteManager().GetSprite(demandIconNames[rowI]);
				row.GetChild(1).gameObject.SetActive(demandIconNames[rowI] != "" && demandIconNames[rowI] != "NONE");
				row.GetChild(2).GetComponent<Text>().text = demandStrings[2 * rowI + 1];
			}
		}
	}
}