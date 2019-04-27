using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvaderAttack : RandomEventSystem.RandomEvent {

	private float mDuration;
	private int mRequiredWarriors;

	public override float Warn() {
		Utilities.LogEvent("Invaders are on the horizon!");
		return(30);
	}
		

	public override void Start () {
		Utilities.LogEvent("Your people are under attack by invaders!");
		mDuration = 30;
		mRequiredWarriors = Random.Range(1, 6);
	}

	public override bool Update () {
		mDuration -= Time.deltaTime;
		if(mDuration <= 0) {
			// maybe this should be in its own method, like Ended()?
			Utilities.LogEvent("The forces have left.");

			int warriorCount = 0;
			// TODO: genericize, move into PersonManager (getting a list of people that satisfy certain criteria will be useful for the UI too)
			List<Person> people = Utilities.GetPersonManager().People;
			foreach(Person p in people) {
				if(p.GetAttribute(Person.AttributeType.PROFESSION) == Person.Attribute.WARRIOR) {
					++warriorCount;
				}
			}

			int warriorDiff = mRequiredWarriors - warriorCount;
			if(warriorDiff < 0) {
				Utilities.LogEvent("Your warriors fended off the invaders and your people took no casualties.");
			}
			else if(warriorDiff > 0) {
				Utilities.LogEvent("Your warriors weren't able to stop all of the invaders.");
				string msg = "";
				int[] hurtPeople = Utilities.RandomList(people.Count, warriorDiff);
				foreach(int i in hurtPeople) {
					people[i].Health -= Random.Range(40,90); // maybe we want to inflict "wounded" instead of directly damaging?
					msg += people[i].Name + " was wounded in the attack.\r\n";
				}
				Utilities.LogEvent(msg);
			}

			return(true);
		}

		return(false);
	}
}
