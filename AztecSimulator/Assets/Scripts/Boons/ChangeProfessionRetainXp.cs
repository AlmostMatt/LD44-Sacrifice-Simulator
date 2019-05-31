using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeProfessionRetainXp : SacrificeResult {

	public class Factory : SacrificeResultFactory {
		public SacrificeResult Make(int tier, int luck) { 
			return new ChangeProfessionRetainXp(tier, luck);
		}
	}

	private int mPercentRetained;
	public ChangeProfessionRetainXp(int tier, int luck) : base("Transferrable Knowledge", "") {
		mPercentRetained = Mathf.Min(100, 10 * (1 + tier + Random.Range(Mathf.Min(6, 1+luck), 6)));
		mDescription = "Your people retain " + mPercentRetained + "% of their xp when changing professions";
        mIcon = "+XP";
	}

	public override void DoEffect()
	{
		GameState.SetBoon(BoonType.PROFESSION_CHANGE_RETAIN_XP, mPercentRetained);
	}
}
