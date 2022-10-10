using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScenarioLibrary {

	private static List<IScenario> sScenarios;

	static ScenarioLibrary()
	{
		// TODO: is there some reflection thing that could be used to only store the type and then instantiate on-demand?
		sScenarios = new List<IScenario>();
        sScenarios.Add(new TutorialScenario());
        sScenarios.Add(new TutorialScenario2());
        sScenarios.Add(new DefaultScenario(
            "Normal Game",
            new List<PersonAttribute> {
                PersonAttribute.FARMER, PersonAttribute.WARRIOR, PersonAttribute.CIVILIAN}));
        sScenarios.Add(new DefaultScenario(
            "Normal Game With Extras",
            PersonAttributeType.PROFESSION.GetAllValues().ToList()));
    }

    public static List<IScenario> Scenarios { get { return sScenarios; } }
}
