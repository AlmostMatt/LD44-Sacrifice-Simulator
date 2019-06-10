using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {
    public GameObject uiScenarioButton;

	// Use this for initialization
	void Awake () {
		foreach (IScenario scenario in ScenarioLibrary.Scenarios)
        {
            GameObject scenarioButton = Instantiate(uiScenarioButton);
            scenarioButton.GetComponent<ScenarioButton>().SetScenario(scenario);
            scenarioButton.transform.SetParent(transform.Find("Center/Content/ScenarioButtons"), false);
        }
	}
}
