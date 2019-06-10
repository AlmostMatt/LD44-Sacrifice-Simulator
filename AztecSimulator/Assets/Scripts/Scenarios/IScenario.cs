using System.Collections.Generic;

public interface IScenario {
    bool IsTutorial { get; }

    string Name { get; }

    SacrificeDemand VictoryDemand { get; }

    List<PersonAttribute> AvailableProfessions { get; }

    List<PersonManager.SpawnPersonRecord> StartingPeople { get; }

    // Maybe: guaranteed boons
    // Maybe: pool of possible boons
    // Maybe: variables related to difficulty
}
