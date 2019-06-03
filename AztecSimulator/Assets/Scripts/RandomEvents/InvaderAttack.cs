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
			: base("Protection", "God will protect your people from the attackers") 
		{
			mAttack = attack;
		}

		public override void DoEffect() {
			mAttack.mIntervened = true;
			GameState.InvaderSize = 0;
		}
	}

	public override float Start () {
		mDuration = 60f;
		int difficultyBoost = Mathf.FloorToInt(GameState.GameTimeElapsed / 90);
		mRequiredWarriors = Random.Range(1, 3) + difficultyBoost;
		GameState.InvaderSize = mRequiredWarriors;

		Utilities.LogEvent("An enemy army approaches! They look to be " + mRequiredWarriors + " strong");
		mOngoing = new Ongoing("ATTACK", "Invaders!", mRequiredWarriors + " enemy warrior" + (mRequiredWarriors == 1 ? " approaches" : "s approach") + "!\r\n(Match their numbers with your army)", mDuration, false);
		GameState.Ongoings.Add(mOngoing);

		mIntervened = false;
		mDemandId = Utilities.GetGod().AddFleetingDemand(
			difficultyBoost,
			new InvaderAttack.GodIntervention(this),
			null, 
			mDuration, 
			"God offers PROTECTION in exchange for "
		); // for now, will just let the god generate the random demand

		return(mDuration);
	}

	public override bool Update () {
		return(mIntervened);
	}

	public override void Removed()
	{
		if(mIntervened)
		{
			Utilities.LogEvent("God intervened and killed the enemies before they attacked.");
		}
		else
		{
			PersonManager personMgr = Utilities.GetPersonManager();

			int warriorStrength = Mathf.FloorToInt(GameState.ArmyStrength);
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
				Utilities.LogEvent(warriorDiff + " invaders made it past your army.");
				int[] hurtPeople = Utilities.RandomList(people.Count, warriorDiff);
				foreach(int i in hurtPeople) {
					int dmg = Random.Range(40,90);
					people[i].Damage(dmg); // maybe we want to inflict "wounded" instead of directly damaging?
					Utilities.LogEvent(people[i].Name + "was wounded by invaders (-" + dmg+ ").", 0f);
				}
			}
		}

		// cleanup things related to the invasion
		if(mDemandId > 0) Utilities.GetGod().RemoveDemand(mDemandId);
		GameState.InvaderSize = 0;

		GameState.Ongoings.Remove(mOngoing);
		mOngoing = null;

		// hack for now to get it to recur
		Utilities.GetEventSystem().ScheduleEvent(new InvaderAttack(), 60);
	}
}
