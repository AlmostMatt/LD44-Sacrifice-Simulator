using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilityBoon : SacrificeResult {

	public FertilityBoon() : base("Fertility Blessing", "Increased birth rate") {
	}

	public override void DoEffect()
	{
		Utilities.LogEvent("Love is in the air");
		GameState.FertilityBonus = 30;
	}
}
