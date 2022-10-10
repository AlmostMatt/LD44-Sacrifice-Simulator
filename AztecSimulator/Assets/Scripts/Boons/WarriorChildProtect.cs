using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorChildProtect : SacrificeResult {

	public class Factory : SacrificeResultFactory {
		public SacrificeResult Make(int tier, int luck) { 
			return new WarriorChildProtect(tier, luck);
		}
	}

	private int mEffectiveness;
	private int mAge;
	private WarriorChildProtect(int tier, int luck) : base("Youngling Protection", "", "WARRIOR") {
		mAge = Random.Range(5, Mathf.Max(11 - luck));
		mEffectiveness = 20 * (1 + tier * 2 + luck);
		mDescription = "Each warrior gets +" + (mEffectiveness / 100f) + " army size for each person of age " + mAge + " or lower";
	}

	public override void DoEffect()
	{
		Utilities.LogEvent("Your warriors are devoted to protecting the youth.", Utilities.SHORT_LOG_DURATION);
		GameState.SetBoon(BoonType.WARRIOR_CHILD_PROTECT_AGE, mAge);
		GameState.SetBoon(BoonType.WARRIOR_CHILD_PROTECT, mEffectiveness);
	}
}
