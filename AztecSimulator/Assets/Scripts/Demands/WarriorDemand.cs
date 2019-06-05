using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorDemand : SacrificeDemand {

	/*
	private int mNumWarriors = 10;

	public WarriorDemand() : base(null, null) {

		Criterion c = new Criterion();
		c.mMinLevel = 20;
		c.mAttributes.Add(PersonAttribute.WARRIOR);
		c.mCount = mNumWarriors;
		mCriteria.Add(c);
	}

	public override bool IsRelevantAttribute(PersonAttribute attribute) {
		return attribute == PersonAttribute.WARRIOR;
	}

	public override bool IsRelevantLevel(int level) {
		return level >= 20;
	}
	
	public override bool CheckSatisfaction(List<Person> people)
	{
		// wants unique people!
		int okPeople = 0;
		foreach(Person p in people)
		{
			if(checkPerson(p))
			{
				++okPeople;
			}
		}

		return(okPeople >= mNumWarriors);
	}

	public override string GetShortDescription() 
	{
		return("10 Level 20 Warriors");
	}

	public override string GetLongDescription() 
	{
		return("VICTORY\r\nDEMAND\r\n10 Level 20 Warriors");
	}

	private bool checkPerson(Person p)
	{
		return(
			(p.GetAttribute(PersonAttributeType.PROFESSION) == PersonAttribute.WARRIOR)
			&&	(p.Level >= 20)
		);
	}
	*/
}
