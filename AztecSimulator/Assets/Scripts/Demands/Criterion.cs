using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Criterion {

	public int mMinAge = -1;
	public int mMinLevel = -1;
	public int mCount = 1;

	public List<Person.Attribute> mAttributes = new List<Person.Attribute>();

	public bool CheckSatisfaction(List<Person> people)
	{
		int satCount = 0;
		foreach(Person p in people)
		{
			if(p.Age >= mMinAge && p.Level >= mMinLevel)
			{
				List<Person.Attribute> unsatisfiedAttrs = new List<Person.Attribute>(mAttributes);
                foreach (Person.Attribute reqAttr in unsatisfiedAttrs)
                {
                    if (p.Profession == reqAttr)
                    {
                        unsatisfiedAttrs.Remove(reqAttr);
                        break;
                    }
				    foreach(Person.Attribute attr in p.NonProfessionAttributes)
				    {
						if(attr == reqAttr)
						{
							unsatisfiedAttrs.Remove(reqAttr);
							break;
						}
				    }
                }

				if(unsatisfiedAttrs.Count == 0)
				{
					++satCount;
				}
			}
		}

		return(satCount >= mCount);
	}

	public bool IsRelevantAttribute(Person.Attribute attribute)
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

	public Person.Attribute GetProfession() {
		foreach (Person.Attribute attr in mAttributes) {
			if (attr.GetAttrType() == Person.AttributeType.PROFESSION) {
				return attr;
			}
		}
		return Person.Attribute.NONE;
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
			mAttributes.FindAll(attr =>attr.GetAttrType() != Person.AttributeType.PROFESSION)
			.ConvertAll(attr => attr.ToString()), false);
		// put a space before the suffix string if there is a suffix and also profession
		if (s!="" && GetProfession() != Person.Attribute.NONE) { s = " " + s; }
		return(s);
	}
}
