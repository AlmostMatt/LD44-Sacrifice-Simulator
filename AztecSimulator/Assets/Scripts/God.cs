using System.Collections;
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

	public string Name { get { return mName;}}
	public List<GodDemand> Demands
	{
		get { return(mDemands); }
	}
	public List<GodDemand> PermanentDemands
	{
		get { return(mDemands.FindAll(x => !x.IsFleeting)); }
	}
	public List<GodDemand> FleetingDemands
	{
		get { return(mDemands.FindAll(x => x.IsFleeting)); }
	}

	void Start () {
		mName = GenerateName();
		mDemands = new List<GodDemand>();
		mFleetingDemandTimer = fleetingDemandTimer;
		mNumFleetingDemands = 0;

		foreach(SacrificeResult sr in BoonLibrary.sGuaranteedRenewableBoons)
		{
			GodDemand renewableDemand = new GodDemand(
				                            DemandGenerator.ScaledDemand(0),
				                            sr,
				                            null
			                            );
			renewableDemand.mIsRenewable = true;
			mDemands.Add(renewableDemand);
		}

		int numTierOneDemands = 2;
		int numTierTwoDemands = 2;
		int numTierThreeDemands = 2;
		SacrificeResultFactory[] boons = BoonLibrary.RandomBoonFactories(6);
		for(int i = 0; i < numTierOneDemands; ++i)
		{
			GodDemand demand = new GodDemand(
				DemandGenerator.ScaledDemand(1),
				boons[i].Make(1, GameState.Favour),
				null
			);
			mDemands.Add(demand);
		}
		for(int i = 0; i < numTierTwoDemands; ++i)
		{
			GodDemand demand = new GodDemand(
				DemandGenerator.ScaledDemand(3),
				boons[numTierOneDemands+i].Make(3, GameState.Favour),
				null
            );
			mDemands.Add(demand);
		}
		for(int i = 0; i < numTierThreeDemands; ++i)
		{
			GodDemand demand = new GodDemand(
				DemandGenerator.ScaledDemand(5),
				boons[numTierOneDemands+numTierTwoDemands+i].Make(5, GameState.Favour),
				null
			);
			mDemands.Add(demand);
		}

		GodDemand victoryDemand = new GodDemand(
			                          DemandGenerator.VictoryDemand(),
			                          new VictoryResult(),
			                          null
		                          );
		mDemands.Add(victoryDemand);

		if(logDemandsOnStart)
		{
			foreach(GodDemand gd in mDemands)
			{
				Utilities.LogEvent("YOUR GOD DEMANDS " + gd.GetShortDescription(), 2f, true);
			}
		}
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
			Utilities.LogEvent(msg + demand.GetShortDescription(), 2f, false);
		}

		return(demand.mId);
	}

	public void RemoveDemand(int demandId)
	{		
		foreach(GodDemand d in mDemands)
		{
			if(d.mId == demandId) {
				mDemands.Remove(d);
				return;
			}
		}
	}

	public List<SacrificeResult> MakeSacrifice(int demandId, List<Person> people) {

		List<SacrificeResult> results = new List<SacrificeResult>();

		if(demandId > 0) {
			GodDemand demand = mDemands.Find(x => x.mId == demandId);
			if(demand == null)
			{
				Debug.Log("No demand with id " + demandId);
				return(results);
			}

			if(demand.mDemand.CheckSatisfaction(people))
			{
				Utilities.LogEvent("YES, THIS SACRIFICE PLEASES ME", 2f, true);
				SacrificeResult sr = demand.mSatisfiedResult;
				if(sr != null)
				{
					results.Add(sr);
				}

				if(demand.mIsRenewable)
				{
					demand.mNumBuys++;
					demand.mDemand = DemandGenerator.ScaledDemand(demand.mNumBuys);
					/*
					if(demand.mNumBuys <= 2)
					{
						demand.mDemand = DemandGenerator.SimpleDemand();
					}
					else if(demand.mNumBuys <= 4)
					{
						demand.mDemand = DemandGenerator.TierOneDemand();
					}
					else
					{
						demand.mDemand = DemandGenerator.TierTwoDemand();	
					}
					*/
				}
				else
				{
					RemoveDemand(demandId);
				}
			}
			else
			{
				Utilities.LogEvent("NO WHAT ARE YOU DOING", 2f, true);
			}
		}
		else {
			Utilities.LogEvent("THIS POINTLESS SACRIFICE PLEASES ME", 2f, true);
		}

		foreach(Person p in people)
		{
			Debug.Log("goodbye " + p.Name);
		}
		PersonManager personMgr = Utilities.GetPersonManager();
		personMgr.RemovePeople(people);

		// for now just apply the results here, whatever...
		foreach(SacrificeResult r in results)
		{
			r.DoEffect();
		}

		if(GameState.HasBoon(BoonType.SACRIFICE_BONUS_XP))
		{
			List<Person> underleveled = personMgr.People.FindAll(x => x.Level < GameState.GetLevelCap(x.GetAttribute(Person.AttributeType.PROFESSION)));
			if(underleveled != null && underleveled.Count > 0)
			{
				int xpBonus = GameState.GetBoonValue(BoonType.SACRIFICE_BONUS_XP);
				Person p = Utilities.RandomSelection<Person>(underleveled.ToArray());
				Utilities.LogEvent(p.Name + " got " + xpBonus + " bonus xp from the sacrifice");
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
				Utilities.LogEvent("The sacrifice restored " + heal + " lifeforce to " + p.Name);
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
