using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerSystem : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		List<Person> people = Utilities.GetPersonManager().People;
		List<Person> hungryPeople = new List<Person>(); // could be optimized with one fixed-length array but w/e
		List<Person> fedPeople = new List<Person>();
		foreach(Person p in people) {
			if(p.Hungry) hungryPeople.Add(p);
			else fedPeople.Add(p);
		}

		int foodSupply = GameState.FoodSupply;
		if(fedPeople.Count > foodSupply)
		{
			// randomly select people to go hungry
			int[] choices = Utilities.RandomList(fedPeople.Count, fedPeople.Count - foodSupply);
			foreach(int i in choices)
			{
				fedPeople[i].Hungry = true;
				Utilities.LogEvent(fedPeople[i].Name + " is starving!", 1f);
			}
		}
		else if(fedPeople.Count < foodSupply && hungryPeople.Count > 0)
		{
			// randomly select people to be fed
			int[] choices = Utilities.RandomList(hungryPeople.Count, Mathf.Min(hungryPeople.Count, foodSupply - fedPeople.Count));
			foreach(int i in choices)
			{
				hungryPeople[i].Hungry = false;
				Utilities.LogEvent(hungryPeople[i].Name + " is no longer starving.", 1f);
			}
		}
	}
}
