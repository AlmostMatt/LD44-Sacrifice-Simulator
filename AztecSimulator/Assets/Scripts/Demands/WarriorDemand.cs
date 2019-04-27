using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorDemand : SacrificeDemand {

	private int mNumWarriors = 10;

	public WarriorDemand() : base(null, null) {
		
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

	public override string GetString()
	{
		return("10 Level 20 Warriors");
	}

	private bool checkPerson(Person p)
	{
		return(
			(p.GetAttribute(Person.AttributeType.PROFESSION) == Person.Attribute.WARRIOR)
			&&	(p.Level >= 20)
		);
	}
}
