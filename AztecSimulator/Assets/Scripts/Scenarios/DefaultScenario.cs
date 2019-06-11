using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DefaultScenario : IScenario
{
    public bool IsTutorial { get { return false; } }
    public string Name { get { return "Normal Game"; } }
    private List<PersonAttribute> mAvailableProfessions;
    public List<PersonAttribute> AvailableProfessions { get { return mAvailableProfessions; } }
    private List<PersonManager.SpawnPersonRecord> mStartingPeople;
    public List<PersonManager.SpawnPersonRecord> StartingPeople { get { return mStartingPeople; } }

    private int mNextDemandGroupId = 0;

    public DefaultScenario()
    {
        mAvailableProfessions = PersonAttributeType.PROFESSION.GetAllValues().ToList();

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
    }

    public List<GodDemand> GenerateInitialDemands()
    {
        // Renewable demands
        List<GodDemand> result = new List<GodDemand>();
        foreach (SacrificeResult sr in BoonLibrary.sGuaranteedRenewableBoons)
        {
            GodDemand renewableDemand = new GodDemand(
                                            DemandGenerator.ScaledDemand(0),
                                            sr,
                                            null
                                        );
            renewableDemand.mIsRenewable = true;
            result.Add(renewableDemand);
        }
        // A group of random demands
        result.AddRange(GenerateDemandGroup(3));
        // The victory demand
        SacrificeDemand victoryDemand = new SacrificeDemand();
        int numToChoose = 1;
        int requiredLevel = 7;
        int requiredCount = 3;
        if (Random.value < 0.5)
        {
            numToChoose = 3;
            requiredLevel = 6;
            requiredCount = 1;
        }
        PersonAttribute[] allProfessions = PersonAttributeType.PROFESSION.GetAllValues();
        PersonAttribute[] randomProfessions = Utilities.RandomSubset<PersonAttribute>(allProfessions, numToChoose);
        foreach (PersonAttribute profession in randomProfessions)
        {
            Criterion profC = new Criterion();
            profC.mMinLevel = requiredLevel;
            profC.mAttributes.Add(profession);
            profC.mCount = requiredCount;
            victoryDemand.mCriteria.Add(profC);
        }
        result.Add(new GodDemand(victoryDemand, new VictoryResult(), null));
        return result;
    }

    // When a normal demand is removed, do nothing
    public List<GodDemand> DemandWasRemoved(GodDemand oldDemand)
    {
        return new List<GodDemand>();
    }

    // When a group is removed, make a new group
    public List<GodDemand> DemandGroupWasRemoved(int groupSize)
    {
        return GenerateDemandGroup(groupSize);
    }

    // Generate a group of demands that will all be removed when any one of them is purchased.
    private List<GodDemand> GenerateDemandGroup(int groupSize)
    {
        List<GodDemand> result = new List<GodDemand>();
        // 3 groups at each tier, starting with tier 0.
        int groupId = mNextDemandGroupId++;
        int tier = Mathf.FloorToInt(groupId / 3f);
        SacrificeResultFactory[] boons = BoonLibrary.RandomBoonFactories(groupSize);
        foreach (SacrificeResultFactory boonFactory in boons)
        {
            GodDemand demand = new GodDemand(
                DemandGenerator.ScaledDemand(tier),
                boonFactory.Make(tier, GameState.Favour),
                null
            );
            demand.GroupId = groupId;
            result.Add(demand);
        }
        return result;
    }
}
