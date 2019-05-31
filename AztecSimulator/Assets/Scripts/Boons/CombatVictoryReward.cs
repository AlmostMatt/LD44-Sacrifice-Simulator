using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatVictoryReward : SacrificeResult {

	public class Factory : SacrificeResultFactory {
		public SacrificeResult Make(int tier, int luck) { 
			return new CombatVictoryReward(tier, luck);
		}
	}

	private BoonType mBoon;
	private int mAmount;

	private CombatVictoryReward(int tier, int luck) : base("Spoils of War: ", "") {
		int whichReward = Random.Range(0, 2);
		switch(whichReward)
		{
		case 0:
			mAmount = Mathf.Min(100, 10 * (tier * 2) + Random.Range(1, 3) * luck);
			mName += "Lifeforce";
			mDescription = "Combat victories restore " + mAmount + " lifeforce to a random person";
            mIcon = "HEALING";
            mBoon = BoonType.COMBAT_VICTORY_BONUS_HEALING;
			break;
		case 1: 
			mAmount = 10 * (tier + luck);
			mName += "Experience";
			mDescription = "Combat victories give a random person +" + mAmount + "xp";
            mIcon = "+XP";
			mBoon = BoonType.COMBAT_VICTORY_BONUS_XP;
			break;
		}
	}

	public override void DoEffect()
	{
		GameState.SetBoon(mBoon, mAmount);
	}
}
