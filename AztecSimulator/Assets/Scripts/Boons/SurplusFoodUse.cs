using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurplusFoodUse : SacrificeResult {

	private bool mToBirthrate = false;
	private string mMsg;

	public SurplusFoodUse() : base("", "") {
		mToBirthrate = Random.value < 0.5f;
		mName = mToBirthrate ? "Baby Food" : "Comfort Food";
		mDescription = mToBirthrate ? "Surplus food increases birthrate" : "Surplus food restores lifeforce";
		mMsg = mToBirthrate ? "Your surplus food is increasing birthrate" : "Your surplus food is restoring lifeforce";
	}

	public override void DoEffect()
	{
		// amount is divided by 100f when retrieved
		Utilities.LogEvent(mMsg);
		if(mToBirthrate)
			GameState.SetBoon(BoonType.SURPLUS_FOOD_TO_BIRTHRATE, 5);
		else
			GameState.SetBoon(BoonType.SURPLUS_FOOD_TO_HEALING, 5);
	}
}
