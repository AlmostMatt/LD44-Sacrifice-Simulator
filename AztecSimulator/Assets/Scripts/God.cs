﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour {

	public bool logDemandsOnStart = false;
	public float fleetingDemandTimer = 30;
	public int maxFleetingDemands = 2;


	private List<GodDemand> mDemands;
	private string mName;
	private float mFleetingDemandTimer;
	private int mNumFleetingDemands;
    private int mNextDemandGroupId = 0;

	public string Name { get { return mName;}}
	public List<GodDemand> Demands
	{
		get { return(mDemands); }
	}
	public List<GodDemand> PermanentDemands
	{
		get { return(mDemands.FindAll(x => !x.IsTemporary)); }
	}
	public List<GodDemand> FleetingDemands
	{
		get { return(mDemands.FindAll(x => x.IsTemporary)); }
	}

	void Start () {
		mName = GenerateName();
		mDemands = new List<GodDemand>();
		mFleetingDemandTimer = fleetingDemandTimer;
		mNumFleetingDemands = 0;

        mDemands.AddRange(GameState.Scenario.GenerateInitialDemands());
        if (logDemandsOnStart)
        {
            foreach (GodDemand gd in mDemands)
            {
                Utilities.LogEvent("YOUR GOD DEMANDS " + gd.GetShortDescription(), Utilities.MEDIUM_LOG_DURATION, true);
            }
        }

		// I don't think we have a specific "game starts now" place
		// but if we get one, maybe this can be moved there
		GameState.Scenario.ScenarioScriptSetup();
    }
	
	// Update is called once per frame
	void Update () {

		if(mNumFleetingDemands < maxFleetingDemands)
		{
			mFleetingDemandTimer -= GameState.GameDeltaTime;
			if(mFleetingDemandTimer <= 0)
			{
				int tier = Mathf.FloorToInt(GameState.GameTimeElapsed / 120);
				SacrificeDemand demand = DemandGenerator.ScaledDemand(tier);
				SacrificeResult satisfied = null;
				SacrificeResult ignored = null;
				float negativeChance = 0.8f - GameState.Favour * 0.1f;
				float specialChance = 0.0f + GameState.Favour * 0.05f;
				float roll = Random.value;
				if(roll <= negativeChance)
					ignored = BoonLibrary.RandomTemporaryCurse();
				else
				if(roll - negativeChance <= specialChance)
					satisfied = BoonLibrary.RandomBoon(tier, GameState.Favour);
				else
					satisfied = BoonLibrary.RandomTemporaryBoon(tier, GameState.Favour);

				GodDemand d = new GodDemand(demand, satisfied, ignored);
				d.mTimeLeft = 30;
				mDemands.Add(d);

				//mFleetingDemands.Add(new FleetingDemand(d, 30));
				mFleetingDemandTimer = Random.Range(fleetingDemandTimer - (fleetingDemandTimer / 2), fleetingDemandTimer + (fleetingDemandTimer / 2));
			}
		}

		for(int i = mDemands.Count - 1; i >= 0; --i)
		{
			GodDemand d = mDemands[i];
			if(d.mTimeLeft == -1)
			{
				// fleeting demands are tacked on to the end, so once we hit a non-fleeting we're done
				break;
			}

			if(d.mTimeLeft > 0)
			{
				d.mTimeLeft -= GameState.GameDeltaTime;
				if(d.mTimeLeft <= 0)
				{
					if(d.mIgnoredResult != null)
					{
						d.mIgnoredResult.DoEffect();
					}
					mDemands.RemoveAt(i);
				}
			}
		}
	}

	public void DebugPrint()
	{
		foreach(GodDemand gd in mDemands)
		{
			Debug.Log("YOUR GOD DEMANDS " + gd.GetShortDescription());
		}
	}

    public int AddFleetingDemand(int tier, SacrificeResult satisfiedResult, SacrificeResult ignoredResult, float time, string msg)
	{
		GodDemand demand = new GodDemand(
			DemandGenerator.ScaledDemand(tier, true),
            satisfiedResult,
            ignoredResult
        );
		demand.mTimeLeft = time;
		mDemands.Add(demand);

		if(msg != null) {
			Utilities.LogEvent(msg + demand.GetShortDescription(), Utilities.MEDIUM_LOG_DURATION, false);
		}

		return(demand.mId);
    }

    // Removes all demands from a group. Returns the size of the group.
    public int RemoveDemandGroup(int groupId)
    {
        return mDemands.RemoveAll(demand => demand.GroupId == groupId);
    }

    public void RemoveDemand(int demandId)
    {
        mDemands.RemoveAll(demand => demand.mId == demandId);
    }

	public List<SacrificeResult> MakeSacrifice(GodDemand demand, List<Person> people) {
		List<SacrificeResult> results = new List<SacrificeResult>();
        int demandId = demand.mId;
        if (demandId < 0 || !demand.mDemand.CheckSatisfaction(people))
        {
            Debug.Log("WARNING: Trying to sacrifice but some requirements not met");
            return results;
        }
        // Sacrifice the people
        foreach (Person p in people)
        {
            Debug.Log("goodbye " + p.Name);
        }
        PersonManager personMgr = Utilities.GetPersonManager();
        personMgr.RemovePeople(people);
        // Apply results after sacrificing people
        if (demand.mSatisfiedResult != null)
        {
            results.Add(demand.mSatisfiedResult);
        }
        if (demand.mSatisfiedResult != null || demand.mIgnoredResult != null)
        {
            Utilities.LogEvent("YES, THIS SACRIFICE PLEASES ME", Utilities.SHORT_LOG_DURATION, true);
        }
        else
        {
            Utilities.LogEvent("THIS POINTLESS SACRIFICE PLEASES ME", Utilities.MEDIUM_LOG_DURATION, true);
        }
        foreach (SacrificeResult r in results)
        {
            r.DoEffect();
        }
        // If a renewable demand was successful and did something, create a new result of the same type
        // This is relevant if the text or any internal variables changed.
        if (demand.mIsRenewable && results.Count > 0)
        {
            demand.mNumBuys++;
            demand.mDemand = DemandGenerator.ScaledDemand(demand.mNumBuys);
            // TODO: find a way to generalize this for renewable demands with arguments
            // var resultType = demand.mSatisfiedResult.GetType();
            // demand.mSatisfiedResult = (SacrificeResult)System.Activator.CreateInstance(resultType);
            // HACK: Assume XPBuff
            XpBuff currentBoon = (XpBuff)demand.mSatisfiedResult;
            demand.mSatisfiedResult = new XpBuff(currentBoon.mProfession);
        }
        // Remove the demand. (Scenario is null if the result was VICTORY)
        if (!demand.mIsRenewable && GameState.Scenario != null)
        {
            if (demand.GroupId != -1)
            {
                int groupSize = RemoveDemandGroup(demand.GroupId);
                mDemands.AddRange(GameState.Scenario.DemandGroupWasRemoved(groupSize));
            }
            else
            {
                RemoveDemand(demandId);
                mDemands.AddRange(GameState.Scenario.DemandWasRemoved(demand));
            }
        }
        if (GameState.HasBoon(BoonType.SACRIFICE_BONUS_XP))
		{
			List<Person> underleveled = personMgr.People.FindAll(x => x.Level < GameState.GetLevelCap(x.GetAttribute(PersonAttributeType.PROFESSION)));
			if(underleveled != null && underleveled.Count > 0)
			{
				int xpBonus = GameState.GetBoonValue(BoonType.SACRIFICE_BONUS_XP);
				Person p = Utilities.RandomSelection<Person>(underleveled.ToArray());
				Utilities.LogEvent(p.Name + " got " + xpBonus + " bonus xp from the sacrifice", Utilities.SHORT_LOG_DURATION);
				p.AddXp(xpBonus);
			}
		}
		if(GameState.HasBoon(BoonType.SACRIFICE_BONUS_HEALING))
		{
			List<Person> needHealing = personMgr.People.FindAll(x => x.Health < x.MaxHealth);
			if(needHealing != null && needHealing.Count > 0)
			{
				float heal = GameState.GetBoonValue(BoonType.SACRIFICE_BONUS_HEALING);
				Person p = Utilities.RandomSelection<Person>(needHealing.ToArray());
				Utilities.LogEvent("The sacrifice restored " + heal + " lifeforce to " + p.Name, Utilities.SHORT_LOG_DURATION);
				p.Heal(heal);
			}
		}
		if(GameState.HasBoon(BoonType.SACRIFICE_BONUS_FOOD))
		{
			// not sure how to do this one yet. would have to be timed to make sense?
		}
		return(results);
	}

	private static string[] sNameParts = {
		"macuil",
		"coz",
		"cacu",
		"auhtli",
		"xochitl",
		"cuetz",
		"ahuia",
		"teteo",
		"centzon",
		"mimix",
		"coa",
		"huitz",
		"nahua",
		"ilop",
		"ochtli",
		"texcat",
		"zonatl",
		"izta",
		"cuhca",
		"cinteotl",
		"cihua",
		"coatl",
		"xiuhi",
		"hue",
		"temaz",
		"calteci",
		"coyotl",
		"ztacuh",
		"qui",
		"micteca",
		"cihuatl",
		"neso",
		"xochi",
		"vpiltzin",
		"tecuhtli",
		"ixquite",
		"catl",
		"itzpa",
		"palotl",
		"totec",
		"chico",
		"malina",
		"cuthli"
	};

	private static string[] sNameSuffixes = {
		
	};

	private string GenerateName()
	{
		string[] parts = Utilities.RandomSubset(sNameParts, 3);
		string name = "";
		foreach(string s in parts)
		{
			name += s;
		}

		name += ", " + (Random.value < 0.5 ? "GOD" : "GODDESS") + " OF AWESOME";

		return name.ToUpper();
	}
}
