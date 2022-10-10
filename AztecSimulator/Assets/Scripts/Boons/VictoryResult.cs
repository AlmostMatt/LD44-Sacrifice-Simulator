using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryResult : SacrificeResult {

	public VictoryResult() : base("VICTORY", "Win the game", "STAR") {
	}

	public override void DoEffect()
	{
		// Set isGod to true to get the same special treatment for this message, whatever it is
		Utilities.LogEvent("GOD HAS BEEN APPEASED. HE LEAVES YOUR PEOPLE IN PEACE", 3f, true);
        // TODO: save the fact that the current scenario has been completed
        // GameState.Victory = true;
        // Tell UI manager to go back to the menu
        Utilities.GetUIManager().GoBackToMenu();
    }
}
