﻿using System.Collections;
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
	private WarriorChildProtect(int tier, int luck) : base("Youngling Protection", "") {
		mAge = Random.Range(5, Mathf.Max(11 - luck));
		mEffectiveness = 20 * (tier * 2 + luck);
		mDescription = "Warriors get +" + (mEffectiveness / 100f) + " effectiveness for each person of age " + mAge + " or lower";
	}

	public override void DoEffect()
	{
		Utilities.LogEvent("Your warriors are devoted to protecting the youth.", 1f);
		GameState.SetBoon(BoonType.WARRIOR_CHILD_PROTECT_AGE, mAge);
		GameState.SetBoon(BoonType.WARRIOR_CHILD_PROTECT, mEffectiveness);
	}
}
