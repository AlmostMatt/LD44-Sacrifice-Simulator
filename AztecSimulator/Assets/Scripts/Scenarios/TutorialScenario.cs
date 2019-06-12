using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScenario : IScenario
{
    public bool IsTutorial { get { return true; } }
    public string Name { get { return "Professions and Sacrifices"; } }
    private List<PersonAttribute> mAvailableProfessions;
    public List<PersonAttribute> AvailableProfessions { get { return mAvailableProfessions; } }
    private List<PersonManager.SpawnPersonRecord> mStartingPeople;
    public List<PersonManager.SpawnPersonRecord> StartingPeople { get { return mStartingPeople; } }

    public TutorialScenario()
    {
        mAvailableProfessions = new List<PersonAttribute> { PersonAttribute.FARMER, PersonAttribute.WARRIOR };

        mStartingPeople = new List<PersonManager.SpawnPersonRecord>();
        // I know these are more verbose compared to defining constructors, but I don't like
        // having to look up the definition of the constructor to figure out what param is what 
        // (especially annoying when the record ends up with a lot of fields).
        PersonManager.SpawnPersonRecord p1 = new PersonManager.SpawnPersonRecord();
        p1.attr = PersonAttribute.FARMER;
        p1.level = 3;
        mStartingPeople.Add(p1);
        PersonManager.SpawnPersonRecord p2 = new PersonManager.SpawnPersonRecord();
        p2.attr = PersonAttribute.FARMER;
        p2.level = 3;
        mStartingPeople.Add(p2);
    }

    public List<GodDemand> GenerateInitialDemands() {
        SacrificeDemand victoryDemand = new SacrificeDemand();
        Criterion profC = new Criterion();
        profC.mMinLevel = 1;
        profC.mAttributes.Add(PersonAttribute.WARRIOR);
        profC.mCount = 1;
        victoryDemand.mCriteria.Add(profC);
        GodDemand victory = new GodDemand(victoryDemand, new VictoryResult(), null);
        return new List<GodDemand>() {victory};
    }

    public List<GodDemand> DemandWasRemoved(GodDemand oldDemand)
    {
        return new List<GodDemand>();
    }

    public List<GodDemand> DemandGroupWasRemoved(int groupSize)
    {
        return new List<GodDemand>();
    }

	public void ScenarioScriptSetup()
	{
		GameObject scriptObject = new GameObject();
		scriptObject.AddComponent<TutorialScript>();

		// uhh is that enough? will it survive? will it leak?
	}
}
