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
	public string mLongDescOverride;

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
			costString += c.GetString() + "\r\n";
		};
		return(costString);
	}

	public string GetLongDescription() {
		if(mLongDescOverride != null)
			return(mLongDescOverride);

		string satisfiedString = mSatisfiedResult == null ? "Fail to satisfy: " + mIgnoredResult.mName : mSatisfiedResult.mName;
		string costString = GetShortDescription();

		return (satisfiedString + "\r\nDEMAND\r\n" + costString).Trim();
	}

	public void DebugPrint()
	{
		Debug.Log(GetShortDescription());
	}
}
