﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour {

	public class FleetingDemand {
		public SacrificeDemand mDemand;
		public float mTimeLeft;

		public FleetingDemand(SacrificeDemand demand, float time)
		{
			mDemand = demand;
			mTimeLeft = time;
		}
	}

	private List<FleetingDemand> mFleetingDemands;
	private List<SacrificeDemand> mDemands;
	private string mName;
	private float mFleetingDemandTimer;

	public string Name { get { return mName;}}
	public List<SacrificeDemand> Demands
	{
		get { return(mDemands); }
	}
	public List<FleetingDemand> FleetingDemands
	{
		get { return(mFleetingDemands); }
	}

	void Start () {
		mName = "MACUILCUETZPALIN, GOD OF AWESOME";
		mDemands = new List<SacrificeDemand>();
		mFleetingDemands = new List<FleetingDemand>();
		mFleetingDemandTimer = 30;

		int numTierOneDemands = 2;
		SacrificeResult[] tierOneBoons = BoonLibrary.RandomTierOneBoons(numTierOneDemands);
		for(int i = 0; i < numTierOneDemands; ++i)
		{
			SacrificeDemand demand = DemandGenerator.TierOneDemand();
			demand.mSatisfiedResult = tierOneBoons[i];
			mDemands.Add(demand);
		}

		SacrificeDemand victoryDemand = DemandGenerator.VictoryDemand();
		victoryDemand.mSatisfiedResult = new VictoryResult();
		mDemands.Add(victoryDemand);

		foreach(SacrificeDemand sd in mDemands)
		{
			Utilities.LogEvent("YOUR GOD DEMANDS " + sd.GetShortDescription());
		}
	}
	
	// Update is called once per frame
	void Update () {
		mFleetingDemandTimer -= Time.deltaTime;
		if(mFleetingDemandTimer <= 0)
		{
			SacrificeDemand d = DemandGenerator.SimpleDemand();
			float negativeChance = 0.8f - GameState.Favour * 0.1f;
			float specialChance = 0.0f + GameState.Favour * 0.05f;
			float roll = Random.value;
			if(roll <= negativeChance)
				d.mIgnoredResult = new PlagueCurse();
			else if(roll - negativeChance <= specialChance)
				d.mSatisfiedResult = BoonLibrary.RandomTierOneBoon();
			else
				d.mSatisfiedResult = BoonLibrary.RandomTemporaryBoon();
			
			mFleetingDemands.Add(new FleetingDemand(d, 120));
			mFleetingDemandTimer = Random.Range(30f,60f);
		}

		for(int i = mFleetingDemands.Count - 1; i >= 0; --i)
		{
			FleetingDemand d = mFleetingDemands[i];
			d.mTimeLeft -= Time.deltaTime;
			if(d.mTimeLeft <= 0)
			{
				mFleetingDemands.RemoveAt(i);
			}
		}
	}

	public void DebugPrint()
	{
		foreach(SacrificeDemand sd in mDemands)
		{
			Debug.Log("YOUR GOD DEMANDS " + sd.GetShortDescription());
		}
	}

	public int AddFleetingDemand(SacrificeResult satisfiedResult, SacrificeResult ignoredResult, float time, string msg)
	{
		SacrificeDemand demand = DemandGenerator.SimpleDemand(satisfiedResult, ignoredResult);
		mFleetingDemands.Add(new FleetingDemand(demand, time));
		if(msg != null) {
			Utilities.LogEvent(msg + demand.GetShortDescription());
		}

		return(demand.mId);
	}

	public void RemoveFleetingDemand(int demandId)
	{
		foreach(FleetingDemand d in mFleetingDemands) {
			if(d.mDemand.mId == demandId) {
				mFleetingDemands.Remove(d);
				return;
			}
		}
	}

	public List<SacrificeResult> MakeSacrifice(int demandId, List<Person> people) {

		List<SacrificeResult> results = new List<SacrificeResult>();

		if(demandId > 0) {
			SacrificeDemand demand;
			FleetingDemand fleetingDemand = mFleetingDemands.Find(x => x.mDemand.mId == demandId);
			if(fleetingDemand != null) {
				demand = fleetingDemand.mDemand;
			} else {
				demand = mDemands.Find(x => x.mId == demandId);	
			}

			if(demand == null)
			{
				Debug.Log("No demand with id " + demandId);
				return(results);
			}

			if(demand.CheckSatisfaction(people))
			{
				Utilities.LogEvent("YES, THIS SACRIFICE PLEASES ME");
				SacrificeResult sr = demand.mSatisfiedResult;
				if(sr != null)
				{
					results.Add(sr);
				}

				mDemands.Remove(demand);
			}
			else
			{
				Utilities.LogEvent("NO WHAT ARE YOU DOING");
			}
		}
		else {
			Utilities.LogEvent("THIS POINTLESS SACRIFICE PLEASES ME");
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

		return(results);
	}
}
