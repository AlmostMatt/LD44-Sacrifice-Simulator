using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryResult : SacrificeResult {

	public VictoryResult() : base("VICTORY", "Win the game") {
	}

	public override void DoEffect()
	{
		Utilities.LogEvent("GOD HAS BEEN APPEASED. HE LEAVES YOUR PEOPLE IN PEACE", 3f);
		// GameState.Victory = true;
	}
}
