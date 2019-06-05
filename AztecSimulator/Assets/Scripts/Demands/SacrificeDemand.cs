using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeDemand
{	
	public List<Criterion> mCriteria;

	public string mShortDescOverride;

	public int NumCriteria
	{
		get { return(mCriteria.Count); }
	}

	public SacrificeDemand() {
		mCriteria = new List<Criterion>();
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

	public bool IsRelevantAttribute(PersonAttribute attribute)
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
			string profString = c.GetProfession() == PersonAttribute.NONE ? "" : c.GetProfession().ToString();
			costString += c.GetPrefixString() + profString + c.GetSuffixString() + "\r\n";
		};
		return(costString);
	}

	// Returns a list of attributes. Corresponding icons will be used in the demand info rows.
	// TODO: support images other than profession icons
	public PersonAttribute[] GetUIDescriptionIcons() {
		PersonAttribute[] attributes = new PersonAttribute[mCriteria.Count];
		for (int i=0; i< mCriteria.Count; i++) {
			attributes[i] = mCriteria[i].GetProfession();
		}
		return attributes;
	}

	public void DebugPrint()
	{
		Debug.Log(GetShortDescription());
	}
}
