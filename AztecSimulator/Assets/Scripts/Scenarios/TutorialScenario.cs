using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScenario : IScenario {

	private List<PersonManager.SpawnPersonRecord> mStartingPeople;

	public List<PersonManager.SpawnPersonRecord> GetStartingPeople() 
	{
		if(mStartingPeople != null) return(mStartingPeople);

		mStartingPeople = new List<PersonManager.SpawnPersonRecord>();

		PersonManager.SpawnPersonRecord p1 = new PersonManager.SpawnPersonRecord();
		p1.attr = Person.Attribute.FARMER;
		//p1.level = 1;
		mStartingPeople.Add(p1);

		PersonManager.SpawnPersonRecord p2 = new PersonManager.SpawnPersonRecord();
		p2.attr = Person.Attribute.FARMER;
		mStartingPeople.Add(p2);

		PersonManager.SpawnPersonRecord p3 = new PersonManager.SpawnPersonRecord();
		p3.attr = Person.Attribute.CIVILIAN;
		//mStartingPeople.Add(p3);

		return(mStartingPeople);
	}
}
