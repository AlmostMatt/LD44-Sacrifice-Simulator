using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour {

	public class SacrificeDemand
	{
		private static int sId = 0;

		public int mId;
		public List<Person.Attribute> mDemandedAttributes;
		public SacrificeResult mSatisfiedResult;
		public SacrificeResult mIgnoredResult;

		public SacrificeDemand(SacrificeResult satisfiedResult, SacrificeResult ignoredResult) {

			mId = ++sId;

			int numAttributes = Random.Range(1,3);
			mDemandedAttributes = GenerateSatisfiableDemands(numAttributes);		

			mSatisfiedResult = satisfiedResult;
			mIgnoredResult = ignoredResult;
		}

		public string GetString()
		{
			string satisfiedString = mSatisfiedResult == null ? "<demanded>" : mSatisfiedResult.mName;
			string costString = GetDemandedAttributes();
			return satisfiedString + "\r\nDEMAND\r\n" + costString;
		}

		public string GetDemandedAttributes()
		{
			return Utilities.ConcatStrings(mDemandedAttributes.ConvertAll(
				attr => System.Enum.GetName(typeof(Person.Attribute), (int)attr)
			), true);
		}

		public void DebugPrint()
		{
			Debug.Log(GetString());
		}

		public static List<Person.Attribute> GenerateSatisfiableDemands(int preferredNum)
		{
			List<Person.Attribute> demands = new List<Person.Attribute>(); 

			// naive (or clever?): if we just pick a person at random and an attribute at random,
			// then we're guaranted to be able to satisfy it. The bonus is that 
			// some other people might share that attribute, so we get player choice involved basically for free
			List<Person> people = Utilities.GetPersonManager().People;

			while(preferredNum-- > 0)
			{
				Person p = people[Random.Range(0, people.Count)];
				Person.Attribute attribute = p.Attributes[Random.Range(0, p.Attributes.Length)];
				if(!demands.Contains(attribute))
					demands.Add(attribute);
			}
			
			return(demands);
		}

		public static List<Person.Attribute> GenerateRandomDemands(int preferredNum)
		{
			List<Person.Attribute> demands = new List<Person.Attribute>();
			var numAttrs = System.Enum.GetValues(typeof(Person.Attribute)).Length-1;
			int[] attributes = Utilities.RandomList(numAttrs, preferredNum);
			for(int i = 0; i < preferredNum; ++i)
			{
				demands.Add((Person.Attribute)attributes[i]);
			}
			return(demands);
		}
	}

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
			mDemands.Add(new SacrificeDemand(new GoodCropBoon(), null));
		}

		DebugPrint();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DebugPrint()
	{
		foreach(SacrificeDemand sd in mDemands)
		{
			Utilities.LogEvent("YOUR GOD DEMANDS A PERSON WITH " + sd.GetDemandedAttributes());
		}
	}

	public int AddDemand(SacrificeResult satisfiedResult, SacrificeResult ignoredResult, string msg)
	{
		SacrificeDemand demand = new SacrificeDemand(satisfiedResult, ignoredResult);
		mDemands.Add(demand);
		if(msg != null) {
			Utilities.LogEvent(msg + demand.GetString());
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

			if(SatisfyDemand(demand, people))
			{
				Utilities.LogEvent("YES, THIS SACRIFICE PLEASES ME");
				SacrificeResult sr = demand.mSatisfiedResult;
				if(sr != null)
				{
					results.Add(sr);
				}

				mDemands.Remove(demand);
				mDemands.Add(new SacrificeDemand(new GoodCropBoon(), null));
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

	private bool SatisfyDemand(SacrificeDemand demand, List<Person> people)
	{
		List<Person.Attribute> demandsCopy = new List<Person.Attribute>(demand.mDemandedAttributes);
		foreach(Person p in people)
		{
			foreach(Person.Attribute attr in p.Attributes)
			{
				foreach(Person.Attribute demandedAttr in demandsCopy)
				{
					if(attr == demandedAttr)
					{
						demandsCopy.Remove(demandedAttr);
						break;
					}
				}
			}
		}

		return(demandsCopy.Count == 0);
	}
}
