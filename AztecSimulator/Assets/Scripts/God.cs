using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour {

	public class GodDemand
	{
		private static int sId = 0;

		public int mId;
		public SacrificeDemand mDemand;
		public SacrificeResult mSatisfiedResult;
		public SacrificeResult mIgnoredResult;

		public float mTimeLeft = -1f;
		public bool mIsRenewable = false;
		public int mNumBuys = 0;

		public string[] mLongDescOverride;

		public GodDemand(SacrificeDemand demand, SacrificeResult satisfiedResult, SacrificeResult ignoredResult)
		{
			mId = ++sId;

			mDemand = demand;
			mSatisfiedResult = satisfiedResult;
			mIgnoredResult = ignoredResult;
		}

		public string GetResultDescription() {
			// todo: consider listing both effect and cost
			return (mSatisfiedResult == null ? mIgnoredResult : mSatisfiedResult).mDescription;
		}

		public string GetShortDescription()
		{
			return(mDemand.GetShortDescription());
		}

		// Returns a list of strings. Two per row, one before the icon, and one after the icon.
		public string[] GetUIDescriptionStrings() {
			if(mLongDescOverride != null) {
				return(mLongDescOverride);
			}

			string[] result = new string[6 + (2 * mDemand.NumCriteria)];

			result[0] = mSatisfiedResult == null ? "GOD THREATENS " : "GOD OFFERS ";
			result[1] = mTimeLeft >= 0 ? string.Format("({0:0.0})", mTimeLeft) : "";
			result[2] = mSatisfiedResult != null ? mSatisfiedResult.mName : mIgnoredResult.mName;
			result[3] = "";
			result[4] = mSatisfiedResult != null ? "In exchange for" : "Unless you pay";
			result[5] = "";
			for (int i=0; i< mDemand.NumCriteria; i++) {
				result[6+(2*i)] = mDemand.mCriteria[i].GetPrefixString();
				result[6+(2*i)+1] = mDemand.mCriteria[i].GetSuffixString();
			}

			return result;
		}

		public Person.Attribute[] GetUIDescriptionIcons() {
			Person.Attribute[] demandAttributes = mDemand.GetUIDescriptionIcons(); 

			Person.Attribute[] attributes = new Person.Attribute[3 + demandAttributes.Length];
			attributes[0] = Person.Attribute.NONE;
			attributes[1] = Person.Attribute.NONE;
			attributes[2] = Person.Attribute.NONE;

			for(int i = 0; i < demandAttributes.Length; ++i)
			{
				attributes[i + 3] = demandAttributes[i];
			}

			return(attributes);
		}
	}

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
		get { return(mDemands.FindAll(x => x.mTimeLeft == -1f)); }
	}
	public List<GodDemand> FleetingDemands
	{
		get { return(mDemands.FindAll(x => x.mTimeLeft > 0)); }
	}

	void Start () {
		mName = "MACUILCUETZPALIN, GOD OF AWESOME";
		mDemands = new List<GodDemand>();
		mFleetingDemandTimer = fleetingDemandTimer;
		mNumFleetingDemands = 0;

		foreach(SacrificeResult sr in BoonLibrary.sGuaranteedRenewableBoons)
		{
			GodDemand renewableDemand = new GodDemand(
				                            DemandGenerator.SimpleDemand(),
				                            sr,
				                            null
			                            );
			renewableDemand.mIsRenewable = true;
			mDemands.Add(renewableDemand);
		}

		int numTierOneDemands = 2;
		SacrificeResult[] tierOneBoons = BoonLibrary.RandomTierOneBoons(numTierOneDemands);
		for(int i = 0; i < numTierOneDemands; ++i)
		{
			GodDemand demand = new GodDemand(
									DemandGenerator.TierOneDemand(),
									tierOneBoons[i],
									null
								);
			mDemands.Add(demand);
		}

		int numTierTwoDemands = 2;
		SacrificeResult[] tierTwoBoons = BoonLibrary.RandomTierTwoBoons(numTierTwoDemands);
		for(int i = 0; i < numTierTwoDemands; ++i)
		{
			GodDemand demand = new GodDemand(
				                   DemandGenerator.TierTwoDemand(),
				                   tierTwoBoons[i],
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
				Utilities.LogEvent("YOUR GOD DEMANDS " + gd.GetShortDescription());
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
				SacrificeDemand demand = DemandGenerator.SimpleDemand();
				SacrificeResult satisfied = null;
				SacrificeResult ignored = null;
				float negativeChance = 0.8f - GameState.Favour * 0.1f;
				float specialChance = 0.0f + GameState.Favour * 0.05f;
				float roll = Random.value;
				if(roll <= negativeChance)
					ignored = BoonLibrary.RandomTemporaryCurse();
				else
				if(roll - negativeChance <= specialChance)
					satisfied = BoonLibrary.RandomTierOneBoon();
				else
					satisfied = BoonLibrary.RandomTemporaryBoon();

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

	public int AddFleetingDemand(SacrificeResult satisfiedResult, SacrificeResult ignoredResult, float time, string msg)
	{
		GodDemand demand = new GodDemand(
			                   DemandGenerator.SimpleDemand(),
			                   satisfiedResult,
			                   ignoredResult
		                   );
		demand.mTimeLeft = time;
		mDemands.Add(demand);

		if(msg != null) {
			Utilities.LogEvent(msg + demand.GetShortDescription());
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
				Utilities.LogEvent("YES, THIS SACRIFICE PLEASES ME", 1f);
				SacrificeResult sr = demand.mSatisfiedResult;
				if(sr != null)
				{
					results.Add(sr);
				}

				if(demand.mIsRenewable)
				{
					demand.mNumBuys++;
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
				}
				else
				{
					RemoveDemand(demandId);
				}
			}
			else
			{
				Utilities.LogEvent("NO WHAT ARE YOU DOING", 1f);
			}
		}
		else {
			Utilities.LogEvent("THIS POINTLESS SACRIFICE PLEASES ME", 1f);
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
}
