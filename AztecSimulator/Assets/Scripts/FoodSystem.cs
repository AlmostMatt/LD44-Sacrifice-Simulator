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

		float totalFoodProduction = 0;
		List<Person> farmers = mPersonMgr.FindPeople(Person.AttributeType.PROFESSION, Person.Attribute.FARMER);
		foreach (Person person in farmers) {
			totalFoodProduction += person.Efficiency;
		}

		int bonusFoodPercent = GameState.GetBoonValue(BoonType.BONUS_FOOD_PERCENT);
        totalFoodProduction *= (100f + bonusFoodPercent) / 100f;

		GameState.FoodSurplus = Mathf.Max(0, totalFoodProduction - mPersonMgr.People.Count);
		GameState.FoodSupply = totalFoodProduction;
	}
}
