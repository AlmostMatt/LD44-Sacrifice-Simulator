using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatVictoryReward : SacrificeResult {

	private BoonType mBoon;
	private int mAmount;

	public CombatVictoryReward() : base("Spoils of War: ", "") {
		int whichReward = Random.Range(0, 2);
		mAmount = Random.Range(1, 5) * 5;
		switch(whichReward)
		{
		case 0:
			mName += "Lifeforce";
			mDescription = "Combat victories heal a random person for " + mAmount;
			mBoon = BoonType.COMBAT_VICTORY_BONUS_HEALING;
			break;
		case 1: 
			mName += " Experience";
			mDescription = "Combat victories give a random person +" + mAmount + "xp";
			mBoon = BoonType.COMBAT_VICTORY_BONUS_XP;
			break;
		}
	}

	public override void DoEffect()
	{
		GameState.SetBoon(mBoon, mAmount);
	}
}
