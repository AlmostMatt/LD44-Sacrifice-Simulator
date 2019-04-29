using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryResult : SacrificeResult {

	public VictoryResult() : base("VICTORY", "Win the game") {
	}

	public override void DoEffect()
	{
		// Set isGod to true to get the same special treatment for this message, whatever it is
		Utilities.LogEvent("GOD HAS BEEN APPEASED. HE LEAVES YOUR PEOPLE IN PEACE", 3f, true);
		// GameState.Victory = true;
	}
}
