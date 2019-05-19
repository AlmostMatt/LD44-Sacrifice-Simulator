using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioLibrary {

	private static Dictionary<string, IScenario> sScenarios;

	static ScenarioLibrary()
	{
		// TODO: is there some reflection thing that could be used to only store the type and then instantiate on-demand?
		sScenarios = new Dictionary<string, IScenario>();
		sScenarios.Add("Default", new DefaultScenario());
		sScenarios.Add("Tutorial", new TutorialScenario());
	}

	public static IScenario Get(string scenarioName)
	{
		return(sScenarios[scenarioName]);
	}
}
