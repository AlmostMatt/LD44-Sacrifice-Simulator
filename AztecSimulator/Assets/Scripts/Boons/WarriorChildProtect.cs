using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorChildProtect : SacrificeResult {

	public WarriorChildProtect() : base("Youngling Protection", "Warriors get +1 for each child in the population (age < 10)") {
	}

	public override void DoEffect()
	{
		Utilities.LogEvent("Your warriors are devoted to protecting the youth.");
		GameState.SetBoon(BoonType.WARRIOR_CHILD_PROTECT, true);
	}
}
