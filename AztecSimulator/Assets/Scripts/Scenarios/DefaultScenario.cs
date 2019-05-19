using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultScenario : IScenario {

	private List<PersonManager.SpawnPersonRecord> mStartingPeople;

	public List<PersonManager.SpawnPersonRecord> GetStartingPeople() 
	{
		if(mStartingPeople != null) return(mStartingPeople);

		mStartingPeople = new List<PersonManager.SpawnPersonRecord>();

		// I know these are more verbose compared to defining constructors, but I don't like
		// having to look up the definition of the constructor to figure out what param is what 
		// (especially annoying when the record ends up with a lot of fields).
		PersonManager.SpawnPersonRecord p1 = new PersonManager.SpawnPersonRecord();
		p1.attr = Person.Attribute.FARMER;
		p1.level = 3;
		mStartingPeople.Add(p1);

		PersonManager.SpawnPersonRecord p2 = new PersonManager.SpawnPersonRecord();
		p2.attr = Person.Attribute.FARMER;
		mStartingPeople.Add(p2);

		PersonManager.SpawnPersonRecord p3 = new PersonManager.SpawnPersonRecord();
		p3.attr = Person.Attribute.CIVILIAN;
		mStartingPeople.Add(p3);

		return(mStartingPeople);
	}
}
