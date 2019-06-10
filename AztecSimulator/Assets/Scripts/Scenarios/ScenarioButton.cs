using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenarioButton : MonoBehaviour {
    private IScenario mScenario;

    public void SetScenario(IScenario scenario)
    {
        mScenario = scenario;
        transform.GetComponentInChildren<Text>().text = scenario.Name;
    }

    public void OnClick()
    {
        if (mScenario != null)
        {
            GameState.Scenario = mScenario;
            if (mScenario.IsTutorial)
            {
                Debug.Log("Tutorial!!");
                // TODO: Consider having the Scene name be a property of the scenario
                SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
            }
            else
            {
                Debug.Log("Normal Level");
                SceneManager.LoadScene("UIScene", LoadSceneMode.Single);
            }
        }
    }
}
