using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurplusFoodUse : SacrificeResult {
	
	public class Factory : SacrificeResultFactory {
		public SacrificeResult Make(int tier, int luck) { 
			return new SurplusFoodUse(tier, luck);
		}
	}

	private bool mToBirthrate = false;
	private string mMsg;

	private int mBoost;
	private SurplusFoodUse(int tier, int luck) : base("", "") {
		mBoost = 5 * (1 + tier + luck);
		mToBirthrate = Random.value < 0.5f;
		mName = mToBirthrate ? "Baby Food" : "Comfort Food";
		mDescription = mToBirthrate ? string.Format("Surplus food increases birthrate by {0:0.00}", (mBoost/100f)) : string.Format("Surplus food restores {0:0.00} lifeforce per second", (mBoost/100f));
		mMsg = mToBirthrate ? "Your surplus food is increasing birthrate" : "Your surplus food is restoring lifeforce";
	}

	public override void DoEffect()
	{
		// amount is divided by 100f when retrieved
		Utilities.LogEvent(mMsg, 1f);
		if(mToBirthrate)
			GameState.SetBoon(BoonType.SURPLUS_FOOD_TO_BIRTHRATE, mBoost);
		else
			GameState.SetBoon(BoonType.SURPLUS_FOOD_TO_HEALING, mBoost);
	}
}
