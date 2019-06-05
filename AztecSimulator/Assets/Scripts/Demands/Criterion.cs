using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Criterion {

	public int mMinAge = -1;
	public int mMinLevel = -1;
	public int mCount = 1;

	public List<PersonAttribute> mAttributes = new List<PersonAttribute>();

    public bool CheckSatisfaction(Person p)
    {
        if (p.Age < mMinAge || p.Level < mMinLevel)
        {
            return false;
        }
        List<PersonAttribute> unsatisfiedAttrs = new List<PersonAttribute>(mAttributes);
        foreach (PersonAttribute reqAttr in mAttributes)
        {
            if (p.Profession == reqAttr)
            {
                unsatisfiedAttrs.Remove(reqAttr);
                continue;
            }
            foreach (PersonAttribute attr in p.NonProfessionAttributes)
            {
                if (attr == reqAttr)
                {
                    unsatisfiedAttrs.Remove(reqAttr);
                    break;
                }
            }
        }
        return unsatisfiedAttrs.Count == 0;
    }

    public bool CheckSatisfaction(List<Person> people)
	{
		int satCount = 0;
		foreach(Person p in people)
		{
            satCount += CheckSatisfaction(p) ? 1 : 0;
        }
		return(satCount >= mCount);
	}

	public bool IsRelevantAttribute(PersonAttribute attribute)
	{
		return(mAttributes.Contains(attribute));
	}

	public bool IsRelevantLevel(int level)
	{
		return(mMinLevel < 0 ? false : level >= mMinLevel);
	}

	public bool IsRelevantAge(int age)
	{
		return(mMinAge < 0 ? false : age >= mMinAge);
	}

	// Returns a string to be shown before the profession
	public string GetPrefixString()
	{
		string s = "";
		//if(mCount > 1) {
			s = mCount + "x ";
		//}
		if(mMinLevel >= 0) {
			s += "Lvl " + mMinLevel + " ";
		}
		return(s);
	}

	public PersonAttribute GetProfession() {
		foreach (PersonAttribute attr in mAttributes) {
			if (attr.GetAttrType() == PersonAttributeType.PROFESSION) {
				return attr;
			}
		}
		return PersonAttribute.NONE;
	}

	//  Returns a string to be shown after the profession
	public string GetSuffixString()
	{
		string s = "";
		if(mMinAge >= 0)
		{
			s += "Age " + mMinAge + " ";
		}
		s += Utilities.ConcatStrings(
			mAttributes.FindAll(attr =>attr.GetAttrType() != PersonAttributeType.PROFESSION)
			.ConvertAll(attr => attr.ToString()), false);
		// put a space before the suffix string if there is a suffix and also profession
		if (s!="" && GetProfession() != PersonAttribute.NONE) { s = " " + s; }
		return(s);
	}
}
