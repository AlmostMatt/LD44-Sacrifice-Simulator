using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSystem : MonoBehaviour {

	private PersonManager mPersonMgr;

	// Use this for initialization
	void Start () {
		mPersonMgr = Utilities.GetPersonManager();
	}
	
	// Update is called once per frame
	void Update () {

		int totalFarmingLevel = 0;
		List<Person> farmers = mPersonMgr.FindPeople(Person.AttributeType.PROFESSION, Person.Attribute.FARMER);
		foreach(Person p in farmers)
		{
			totalFarmingLevel += p.Level;
		}

		GameState.FoodSupply = totalFarmingLevel;
	}
}
