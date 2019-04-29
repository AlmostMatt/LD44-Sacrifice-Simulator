using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeProfessionRetainXp : SacrificeResult {

	private int mPercentRetained;

	public ChangeProfessionRetainXp() : base("Transferrable Knowledge", "") {
		mPercentRetained = Random.Range(1, 6) * 10;
		mDescription = "Your people retain " + mPercentRetained + "% of their xp when changing professions";
	}

	public override void DoEffect()
	{
		GameState.SetBoon(BoonType.PROFESSION_CHANGE_RETAIN_XP, mPercentRetained);
	}
}
