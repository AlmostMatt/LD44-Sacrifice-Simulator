using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryResult : SacrificeResult {

	public VictoryResult() : base("VICTORY", "Win the game", "STAR") {
	}

	public override void DoEffect()
	{
		// Set isGod to true to get the same special treatment for this message, whatever it is
		Utilities.LogEvent("GOD HAS BEEN APPEASED. HE LEAVES YOUR PEOPLE IN PEACE", 3f, true);
        // TODO: save the fact that the current scenario has been completed
        // GameState.Victory = true;
        // Go back to the menu after 2 seconds
        // TODO: add a short delay (Invoke and Coroutine probably both require a MonoBehaviour)
        BackToMenu();
    }

    private void BackToMenu()
    {
        GameState.Scenario = null;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
