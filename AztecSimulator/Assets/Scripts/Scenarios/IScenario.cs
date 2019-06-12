using System.Collections.Generic;

public interface IScenario {
    bool IsTutorial { get; }

    string Name { get; }

    List<PersonAttribute> AvailableProfessions { get; }

    List<PersonManager.SpawnPersonRecord> StartingPeople { get; }

    List<GodDemand> GenerateInitialDemands();

    List<GodDemand> DemandWasRemoved(GodDemand oldDemand);

    List<GodDemand> DemandGroupWasRemoved(int groupSize);

	void ScenarioScriptSetup();

    // Maybe: variables related to difficulty
}
