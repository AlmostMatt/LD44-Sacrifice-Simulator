using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeDemand
{
	private static int sId = 0;

	public int mId;
	public List<Criterion> mCriteria;
	public SacrificeResult mSatisfiedResult;
	public SacrificeResult mIgnoredResult;

	public string mShortDescOverride;
	public string[] mLongDescOverride;

	public SacrificeDemand(SacrificeResult satisfiedResult, SacrificeResult ignoredResult) {

		mId = ++sId;

		mCriteria = new List<Criterion>();
		mSatisfiedResult = satisfiedResult;
		mIgnoredResult = ignoredResult;
	}

	public SacrificeDemand() : this(null, null) {
	}

	public bool CheckSatisfaction(List<Person> people)
	{
		foreach(Criterion c in mCriteria)
		{
			if(!c.CheckSatisfaction(people))
				return(false);
		}

		return(true);
	}

	public bool IsRelevantAttribute(Person.Attribute attribute)
	{
		foreach(Criterion c in mCriteria)
		{
			if(c.IsRelevantAttribute(attribute))
				return(true);
		}
		return(false);
	}

	public bool IsRelevantLevel(int level)
	{
		foreach(Criterion c in mCriteria)
		{
			if(c.IsRelevantLevel(level))
				return(true);
		}
		return(false);
	}

	public bool IsRelevantAge(int age)
	{
		foreach(Criterion c in mCriteria)
		{
			if(c.IsRelevantAge(age))
				return(true);
		}
		return(false);
	}

	public string GetShortDescription() {
		if(mShortDescOverride != null)
			return(mShortDescOverride);	

		string costString = "";
		foreach(Criterion c in mCriteria)
		{
			string profString = c.GetProfession() == Person.Attribute.NONE ? "" : c.GetProfession().ToString();
			costString += c.GetPrefixString() + profString + c.GetSuffixString() + "\r\n";
		};
		return(costString);
	}

	// Returns a list of strings. Two per row, one before the icon, and one after the icon.
	public string[] GetUIDescriptionStrings() {
		if(mLongDescOverride != null) {
			return(mLongDescOverride);
		}
		string[] result = new string[4 + (2 * mCriteria.Count)];
		string satisfiedString = mSatisfiedResult == null ? "Fail to satisfy: " + mIgnoredResult.mName : mSatisfiedResult.mName;
		result[0] = "";
		result[1] = satisfiedString;
		result[2] = "DEMAND";
		result[3] = "";
		for (int i=0; i< mCriteria.Count; i++) {
			result[4+(2*i)] = mCriteria[i].GetPrefixString();
			result[4+(2*i)+1] = mCriteria[i].GetSuffixString();
		}
		return result;
	}

	// Returns a list of attributes. Corresponding icons will be used in the demand info rows.
	// TODO: support images other than profession icons
	public Person.Attribute[] GetUIDescriptionIcons() {
		Person.Attribute[] attributes = new Person.Attribute[2 + mCriteria.Count];
		attributes[0] = Person.Attribute.WARRIOR;
		attributes[1] = Person.Attribute.NONE;
		for (int i=0; i< mCriteria.Count; i++) {
			attributes[2+i] = mCriteria[i].GetProfession();
		}
		return attributes;
	}

	public void DebugPrint()
	{
		Debug.Log(GetShortDescription());
	}
}
