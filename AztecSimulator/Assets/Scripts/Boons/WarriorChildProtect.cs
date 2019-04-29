using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorChildProtect : SacrificeResult {

	private int mAge;

	public WarriorChildProtect() : base("Youngling Protection", "") {
		mAge = Random.Range(5, 11);
		mDescription = "Warriors get +1 army for each person of age " + mAge + " or lower";
	}

	public override void DoEffect()
	{
		Utilities.LogEvent("Your warriors are devoted to protecting the youth.", 1f);
		GameState.SetBoon(BoonType.WARRIOR_CHILD_PROTECT, mAge);
	}
}
