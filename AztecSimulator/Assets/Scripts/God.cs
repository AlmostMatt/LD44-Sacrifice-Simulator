using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour {

	private List<SacrificeDemand> mDemands;
	private string mName;

	public string Name { get { return mName;}}
	public List<SacrificeDemand> Demands
	{
		get { return(mDemands); }
	}

	// Use this for initialization
	void Start () {
		mName = "MACUILCUETZPALIN, GOD OF AWESOME";
		mDemands = new List<SacrificeDemand>();

		int numDemands = 1; // Random.Range(1, 3);
		for(int i = 0; i < numDemands; ++i)
		{
			mDemands.Add(new SimpleDemand(new GoodCropBoon(), null));
		}

		mDemands.Add(new WarriorDemand());

		DebugPrint();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DebugPrint()
	{
		foreach(SacrificeDemand sd in mDemands)
		{
			Utilities.LogEvent("YOUR GOD DEMANDS " + sd.GetShortDescription());
		}
	}

	public int AddDemand(SacrificeResult satisfiedResult, SacrificeResult ignoredResult, string msg)
	{
		SacrificeDemand demand = new SimpleDemand(satisfiedResult, ignoredResult);
		mDemands.Add(demand);
		if(msg != null) {
			Utilities.LogEvent(msg + demand.GetShortDescription());
		}

		return(demand.mId);
	}

	public void RemoveDemand(int demandId)
	{
		foreach(SacrificeDemand d in mDemands) {
			if(d.mId == demandId) {
				mDemands.Remove(d);
				return;
			}
		}
	}

	public List<SacrificeResult> MakeSacrifice(int demandId, List<Person> people) {

		List<SacrificeResult> results = new List<SacrificeResult>();

		if(demandId > 0) {
			SacrificeDemand demand = mDemands.Find(x => x.mId == demandId);
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
				mDemands.Add(new SimpleDemand(new GoodCropBoon(), null));
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

		DebugPrint(); // temp hack: print out new demands (after other messages happen)

		return(results);
	}
}
