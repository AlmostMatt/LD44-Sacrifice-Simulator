﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioLibrary {

	private static List<IScenario> sScenarios;

	static ScenarioLibrary()
	{
		// TODO: is there some reflection thing that could be used to only store the type and then instantiate on-demand?
		sScenarios = new List<IScenario>();
        sScenarios.Add(new TutorialScenario());
        sScenarios.Add(new TutorialScenario2());
        sScenarios.Add(new Scenario1());
        sScenarios.Add(new DefaultScenario());
    }

    public static List<IScenario> Scenarios { get { return sScenarios; } }
}
