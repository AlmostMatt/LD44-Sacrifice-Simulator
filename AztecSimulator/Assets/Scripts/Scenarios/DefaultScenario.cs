using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DefaultScenario : IScenario
{
    public bool IsTutorial { get { return false; } }
    public string Name { get { return "Normal Game"; } }
    private SacrificeDemand mVictoryDemand;
    public SacrificeDemand VictoryDemand { get { return mVictoryDemand; } }
    private List<PersonAttribute> mAvailableProfessions;
    public List<PersonAttribute> AvailableProfessions { get { return mAvailableProfessions; } }
    private List<PersonManager.SpawnPersonRecord> mStartingPeople;
    public List<PersonManager.SpawnPersonRecord> StartingPeople { get { return mStartingPeople; } }

    public DefaultScenario()
    {
        PersonAttribute[] allProfessions = PersonAttributeType.PROFESSION.GetAllValues();
        mAvailableProfessions = allProfessions.ToList();

        mStartingPeople = new List<PersonManager.SpawnPersonRecord>();
        // I know these are more verbose compared to defining constructors, but I don't like
        // having to look up the definition of the constructor to figure out what param is what 
        // (especially annoying when the record ends up with a lot of fields).
        PersonManager.SpawnPersonRecord p1 = new PersonManager.SpawnPersonRecord();
        p1.attr = PersonAttribute.FARMER;
        p1.level = 3;
        mStartingPeople.Add(p1);
        PersonManager.SpawnPersonRecord p2 = new PersonManager.SpawnPersonRecord();
        p2.attr = PersonAttribute.CIVILIAN;
        mStartingPeople.Add(p2);
        PersonManager.SpawnPersonRecord p3 = new PersonManager.SpawnPersonRecord();
        p3.attr = PersonAttribute.CIVILIAN;
        mStartingPeople.Add(p3);

        mVictoryDemand = new SacrificeDemand();
        int numToChoose = 1;
        int requiredLevel = 7;
        int requiredCount = 3;
        if (Random.value < 0.5)
        {
            numToChoose = 3;
            requiredLevel = 6;
            requiredCount = 1;
        }
        PersonAttribute[] randomProfessions = Utilities.RandomSubset<PersonAttribute>(allProfessions, numToChoose);
        foreach (PersonAttribute profession in randomProfessions)
        {
            Criterion profC = new Criterion();
            profC.mMinLevel = requiredLevel;
            profC.mAttributes.Add(profession);
            profC.mCount = requiredCount;
            mVictoryDemand.mCriteria.Add(profC);
        }
    }
}
