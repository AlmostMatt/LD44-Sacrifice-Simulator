using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour {

	public string scenarioName;

	// Use this for initialization
	void Start () {
		GameState.Scenario = ScenarioLibrary.Get(scenarioName);
	}
}
