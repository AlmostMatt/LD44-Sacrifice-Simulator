using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvaderAttack : RandomEventSystem.RandomEvent {

	private float mDuration;
	private int mRequiredWarriors;
	private bool mIntervened;
	private int mDemandId;

	private class GodIntervention : SacrificeResult {
		private InvaderAttack mAttack;

		public GodIntervention(InvaderAttack attack) 
			: base("Divine Intervention - Protection", "God will protect your people from the attackers") 
		{
			mAttack = attack;
		}

		public override void DoEffect() {
			mAttack.mIntervened = true;
			GameState.InvaderSize = 0;
		}
	}

	public override float Warn() {
		mRequiredWarriors = Random.Range(3, 10);
		GameState.InvaderSize = mRequiredWarriors;
		Utilities.LogEvent("An enemy army approaches! They look to be about " + mRequiredWarriors + " strong");
		return(30);
	}

	public override void Start () {
		Utilities.LogEvent("Your people are under attack by invaders!");
		mDemandId = 0;
		mIntervened = false;
		mDuration = 30;

	//	if(Random.value < 0.7) {
			mDemandId = Utilities.GetGod().AddFleetingDemand(
				new InvaderAttack.GodIntervention(this),
				null, 
				mDuration, 
				"God offers PROTECTION in exchange for "
			); // for now, will just let the god generate the random demand
	//	}
	}

	public override bool Update () {
		
		if(mIntervened)
		{
			Utilities.LogEvent("God intervened and stopped the attack. No one died!");
			return(true);
		}

		if(mDuration > 0)
		{
			mDuration -= GameState.GameDeltaTime;
			if(mDuration <= 0) {
				// maybe this should be in its own method, like Ended()?
				PersonManager personMgr = Utilities.GetPersonManager();

				int warriorStrength = GameState.ArmyStrength;
				int warriorDiff = mRequiredWarriors - warriorStrength;
				if(warriorDiff <= 0) {
					Utilities.LogEvent("Your warriors fended off the invaders and your people took no casualties.");

					int bonusHealing = GameState.GetBoonValue(BoonType.COMBAT_VICTORY_BONUS_HEALING);
					if(bonusHealing > 0) {
						List<Person> peopleToHeal = personMgr.People.FindAll(x => x.Health < x.MaxHealth);
						if(peopleToHeal.Count > 0)
						{
							Person p = Utilities.RandomSelection<Person>(peopleToHeal.ToArray());
							p.Heal(bonusHealing);
							Utilities.LogEvent(p.Name + " feels revitalized by the victory. +" + bonusHealing + " lifeforce");
						}
					}

					int bonusXp = GameState.GetBoonValue(BoonType.COMBAT_VICTORY_BONUS_XP);
					if(bonusXp > 0) {
						List<Person> peopleToGetXp = personMgr.People.FindAll(x => x.Level < GameState.GetLevelCap(x.GetAttribute(Person.AttributeType.PROFESSION)));
						if(peopleToGetXp.Count > 0)
						{
							Person p = Utilities.RandomSelection<Person>(peopleToGetXp.ToArray());
							p.AddXp(bonusXp);
							Utilities.LogEvent(p.Name + " has learned from the victory! +" + bonusXp + "xp");
						}
					}
				}
				else if(warriorDiff > 0) {
					List<Person> people = personMgr.People;
					Utilities.LogEvent("Your warriors weren't able to stop all of the invaders.");
					string msg = "";
					int[] hurtPeople = Utilities.RandomList(people.Count, warriorDiff);
					foreach(int i in hurtPeople) {
						people[i].Health -= Random.Range(40,90); // maybe we want to inflict "wounded" instead of directly damaging?
						msg += people[i].Name + " was wounded in the attack.\r\n";
					}
					Utilities.LogEvent(msg);
				}

				// cleanup things related to the invasion
				if(mDemandId > 0) Utilities.GetGod().RemoveDemand(mDemandId);
				GameState.InvaderSize = 0;

				return(true);
			}
		}

		return(false);
	}

	public override void Removed()
	{
		// hack for now to get it to recur
		Utilities.GetEventSystem().ScheduleEvent(new InvaderAttack(), 120);
	}
}
