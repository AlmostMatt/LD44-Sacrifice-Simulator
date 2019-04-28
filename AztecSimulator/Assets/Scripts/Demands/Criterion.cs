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
				foreach(Person.Attribute attr in p.Attributes)
				{
					foreach(Person.Attribute reqAttr in unsatisfiedAttrs)
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

	public string GetString()
	{
		string s = "";
		if(mCount > 1)
		{
			s = mCount + " ";
		}

		if(mMinAge >= 0)
		{
			s += "Age " + mMinAge + " ";
		}

		if(mMinLevel >= 0)
		{
			s += "Level " + mMinLevel + " ";
		}
			
		s += Utilities.ConcatStrings(mAttributes.ConvertAll(
			attr => System.Enum.GetName(typeof(Person.Attribute), (int)attr)
		), true);

		return(s);
	}

}
